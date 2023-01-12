using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp11Demo.example
{
    ref struct ReadonlyRef
    {
        /// <summary>
        /// 一个 int 的引用
        /// </summary>
        public ref int filed1;

        /// <summary>
        /// 一个 int 的只读引用
        /// </summary>
        public readonly ref int filed2;

        /// <summary>
        /// 一个只读 int 的引用
        /// </summary>
        public ref readonly  int filed3;

        /// <summary>
        /// 一个只读 int 的只读引用
        /// </summary>
        public readonly ref readonly int filed4;

    }
}
