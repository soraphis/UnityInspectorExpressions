using System;
using System.Collections.Generic;
using System.Linq;

namespace Editor.Helpers
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Type> GetAllGenericImplementationsTypes(object obj, Type constraint)
        {
            if (obj == null)
            {
                return Enumerable.Empty<Type>();
            }

            var objType = obj.GetType();
            var interfaces = objType.GetInterfaces()
                                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == constraint);

            return interfaces.Select(i => i.GetGenericArguments()[0]);
        }

    }
}
