using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Puresharp.SimpleInjectorBattle
{
    public class Benchmark
    {
        static Benchmark()
        {
            var _seed = Environment.TickCount;
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        static private string Native = "[native]";

        static private string Round(int duration)
        {
            var _duration = duration.ToString();
            if (_duration.Length == 2)
            {
                if (int.Parse(_duration[1].ToString()) < 5) { _duration = $"{ _duration[0] }0"; }
                else { _duration = $"{ int.Parse(_duration[0].ToString()) + 1 }0"; }
            }
            else if (_duration.Length > 2) { _duration = $"{ _duration[0] }{ _duration[1] }".PadRight(_duration.Length, '0'); }
            return int.Parse(_duration).ToString("N0");
        }

        private Dictionary<string, Func<Action>> m_Dictionary;

        public Benchmark(Func<Action> native)
        {
            this.m_Dictionary = new Dictionary<string, Func<Action>>();
            this.m_Dictionary.Add(Benchmark.Native, native);
        }

        public void Add(string name, Func<Action> alternative)
        {
            this.m_Dictionary.Add(name, alternative);
        }

        public Dictionary<string, int> Run(Action<string> log)
        {
            var _log = log != null;
            var _stopwatch = new Stopwatch();
            var _activation = this.m_Dictionary[Benchmark.Native];
            var _action = _activation();
            var _iteration = 1;
            _action();
            _action();
            _action();
            while (true)
            {
                var _index = 0;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                _stopwatch.Restart();
                while (_index++ < _iteration) { _action(); }
                _stopwatch.Stop();
                if (_stopwatch.ElapsedMilliseconds < 100) { _iteration = _iteration * 10; }
                else { break; }
            }
            if (_log) { log($"Benchmark : iteration = { _iteration }"); }
            var _list = new List<KeyValuePair<string, long>>();
            var _array = this.m_Dictionary.ToArray();
            var _buffer = new KeyValuePair<string, Action>[_array.Length];
            if (_log) { log("    Activation"); }
            for (var _loop = 0; _loop < _array.Length; _loop++)
            {
                var _name = _array[_loop].Key;
                var _activate = _array[_loop].Value;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                _stopwatch.Restart();
                _action = _activate();
                _stopwatch.Stop();
                _buffer[_loop] = new KeyValuePair<string, Action>(_name, _action);
                if (_log) { log($"        { _name } = { _stopwatch.ElapsedMilliseconds }"); }
            }
            if (_log) { log("    Warmup"); }
            foreach (var _item in _buffer)
            {
                _action = _item.Value;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                _stopwatch.Restart();
                _action();
                _stopwatch.Stop();
                if (_log) { log($"        { _item.Key } = { _stopwatch.ElapsedMilliseconds }"); }
            }
            if (_log) { log("    Loop"); }
            for (var _loop = 0; _loop < 10; _loop++)
            {
                foreach (var _item in _buffer)
                {
                    var _index = 0;
                    _action = _item.Value;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    _stopwatch.Restart();
                    while (_index++ < 100) { _action(); }
                    _stopwatch.Stop();
                }
            }
            foreach (var _item in _buffer)
            {
                var _index = 0;
                _action = _item.Value;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                _stopwatch.Restart();
                while (_index++ < _iteration) { _action(); }
                _stopwatch.Stop();
                if (_log) { log($"        { _item.Key } = { _stopwatch.ElapsedMilliseconds }"); }
                _list.Add(new KeyValuePair<string, long>(_item.Key, _stopwatch.ElapsedTicks));
            }
            var _dashboard = _list.GroupBy(_Measure => _Measure.Key, _Measure => _Measure.Value).Select(_Measure => new { Name = _Measure.Key, Duration = _Measure.Average() }).ToArray();
            var _native = _dashboard.Single(_Measure => _Measure.Name == Benchmark.Native).Duration;
            var _dictionary = _dashboard.ToDictionary(_Measure => _Measure.Name, _Measure => Convert.ToInt32((_Measure.Duration * 100) / _native));
            if (_log)
            {
                var _max = _dictionary.Select(_Measure => _Measure.Key.Length).Max();
                var _ddd = _dictionary.OrderBy(_Measure => _Measure.Value * (_Measure.Key == Benchmark.Native ? 0 : 1)).ThenBy(_Measure => _Measure.Key).Select(_Measure => new { Name = _Measure.Key, Duration = Benchmark.Round(_Measure.Value) }).ToArray();
                var _mmm = _ddd.Select(_Measure => _Measure.Duration.Length).Max();
                var _size = _ddd.Count().ToString().Length;
                var _line = new string('=', _max + _mmm + 8 + _size);
                var _index = -1;
                Console.WriteLine();
                Console.WriteLine($" { _line }");
                foreach (var _measure in _ddd) { Console.WriteLine($" { (++_index == 0 ? new string(' ', _size + 1) : string.Concat("[", _index, "]")) } { _measure.Name.PadLeft(_max, ' ') } : { _measure.Duration.PadLeft(_mmm) } %"); }
                Console.WriteLine($" { _line }");
            }
            return _dictionary;
        }
    }
}
