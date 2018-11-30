using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Reflection {
    class Program {
        static void Main(string[] args) {
            var myObj = new MyClass();
            myObj.Name = "test2";
            myObj.Value = 32;

            Serialize(new { Prop = "text", Val = 34, Props = new String[] { "s1", "s2", "s3" }, Obj = new MyClass(), Obj2 = myObj });

            Console.ReadLine();
        }

        private static void OutputDepth(object o, int depth) {
            for (int i = 0; i < depth; i++) {
                Console.Write("    ");
            }

            Console.WriteLine(o.ToString());
        }

        private static void Serialize(object p, int maxDepth = 3, int depth = 0) {
            if (depth > maxDepth) {
                return;
            }

            var type = p.GetType();
            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) {
                if (field.IsInitOnly || field.Name.Contains("BackingField")) {
                    continue;
                }

                OutputDepth(field.Name, depth);
            }

            var properties = type.GetProperties();
            foreach (var pro in properties) {
                OutputDepth(pro.Name, depth);

                if (pro.PropertyType.IsArray) {
                    foreach (var item in (Array)pro.GetValue(p)) {
                        OutputDepth(item, depth + 1);
                    }
                } else if (pro.PropertyType == typeof(string)) {
                    OutputDepth(pro.GetValue(p), depth + 1);
                } else if (pro.PropertyType.IsPrimitive) {
                    if (!pro.CanRead) {
                        continue;
                    }

                    OutputDepth(pro.GetValue(p), depth + 1);
                } else if (pro.PropertyType.IsClass) {
                    Serialize(pro.GetValue(p), maxDepth, depth + 1);
                }

            }
        }
    }

    public class MyClass {
        private int value = 42;
        public string Name { get; set; } = "test";
        public bool Bool => true;
        public int Value { set { this.value = value; } }
    }
}
