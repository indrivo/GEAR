using System.Collections.Generic;

namespace ST.Identity.Data.Permissions
{
    public class PermissionEqualityComparer : IEqualityComparer<PermissionUserToExecute>
    {
        /// <inheritdoc />
        /// <summary>
        /// Check whether two permissions are equal.
        /// They are equal only once their Service, Module and 
        /// Action are equal.
        /// </summary>
        /// <param name="x">First permission operand</param>
        /// <param name="y">Second permission operand</param>
        /// <returns>True if objects are equal</returns>
        public bool Equals(PermissionUserToExecute x, PermissionUserToExecute y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Service.Equals(y.Service)
                   && x.Module.Equals(y.Module)
                   && x.Action.Equals(y.Action);
        }

        /// <inheritdoc />
        /// <summary>
        /// This will be useful when using Linq
        /// Methods that deal with comparing equality
        /// of two objects, or storing them into a Hashed 
        /// enumerable. e.g. 
        /// <see cref="M:System.Linq.Enumerable.Distinct``1(System.Collections.Generic.IEnumerable{``0})" />,
        /// <see cref="M:System.Linq.Enumerable.Intersect``1(System.Collections.Generic.IEnumerable{``0},System.Collections.Generic.IEnumerable{``0})" />
        /// or adding these object to a <see cref="T:System.Collections.Generic.HashSet`1" /> so they are unique.
        /// </summary>
        /// <param name="obj">The permission object to get hashcode</param>
        /// <returns>Hash code of the current object</returns>
        public int GetHashCode(PermissionUserToExecute obj)
        {
            return obj.Service.GetHashCode()
                   ^ obj.Module.GetHashCode()
                   ^ obj.Action.GetHashCode();
        }
    }
}