using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoadFileData
{
    public class GenericExpando : DynamicObject
    {
        private readonly object instance = null;
        private readonly Type staticType = null;

        public GenericExpando(Type staticType)
        {
            this.staticType = staticType;
        }

        public GenericExpando(object instance)
        {
            this.instance = instance;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            if (args.Length < 1)
            {
                return false;
            }
            if (args[0] == null)
            {
                return false;
            }

            Type[] genericTypes = null;
            var type = args[0] as Type;
            if (type != null)
            {
                genericTypes = new[] { type };
            }
            var types = args[0] as Type[];
            if (types != null)
            {
                genericTypes = types;
            }
            if ((genericTypes == null) || (genericTypes.Length == 0))
            {
                return false;
            }
            var callingArgs = new object[args.Length - 1];
            for (var i = 1; i < args.Length; i++)
            {
                callingArgs[i - 1] = args[i];
            }
            if (staticType != null)
            {
                result = GenericHelper.InvokeStatic(
                    binder.Name,
                    staticType,
                    genericTypes,
                    callingArgs);
                return true;
            }
            if (instance == null)
            {
                return false;
            }

            result = GenericHelper.InvokeInstance(
                binder.Name,
                instance,
                genericTypes,
                callingArgs);
            return true;
        }
    }
}
