using System;
using System.Diagnostics;

namespace ST.Identity.Data.Permissions
{
    /// <summary>
    /// Represents an ability of a user to execute
    /// an action relative to it's module and service context
    /// </summary>
    [DebuggerDisplay(@"\{{Service,nq}:{Module,nq}:{Action,nq}\}")]
    public class PermissionUserToExecute
    {
        /// <summary>
        /// Permission's format, aka string representation of this permission
        /// {Service:Module:Action}
        /// Service - The micro-service this permission works with
        /// Module  - A specific module of that service that needs permission checks
        /// Action  - An action that the user will execute after permission checks have passed
        /// e.g.:
        /// Identity:Users:Create
        /// </summary>
        private const string PermissionFormat = "{0}\x003A{1}\x003A{2}";

        /// <summary>
        /// The permission separator character which by default is the
        /// colon character with code U+003A (:)
        /// </summary>
        private const char PermissionSeparator = '\x003A';

        /// <summary>
        /// The action user wants to execute on the <see cref="Module"/>
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Short description of what does this permission do.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The module which is part of the <see cref="Service"/>
        /// user requests access.
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// The service where this permission works on
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// Useful to construct a <see cref="Permission"/> object
        /// using a formatted string representation. In this case the
        /// Permission should be formatted in this way: 'Service:Module:Action'
        /// e.g. 'Identity:Administration:CreateUser
        /// </summary>
        /// <param name="permissionFormatted">Formatted permission representation</param>
        /// <exception cref="FormatException">
        /// Thrown when the input string does not conform to the specified format.
        /// </exception>
        public static implicit operator PermissionUserToExecute(string permissionFormatted) =>
            ToPermissionObject(permissionFormatted);

        /// <summary>
        /// Conversion operator to convert Permission object to string, can
        /// be used to cast permission object to a string representation.
        /// </summary>
        /// <param name="permission">The permission to convert</param>
        /// <exception cref="ArgumentNullException">
        /// Can be thrown by <see cref="string.Format(string, object[])"/>
        /// </exception>
        public static implicit operator string(PermissionUserToExecute permission) =>
            ToPermissionCode(permission);

        /// <summary>
        /// Get's the PermissionCode, a string representation of this Permission
        /// object that can be added in database in groups, users e.t.c. to denote that
        /// the specified entity has this permission to execute an action.
        /// </summary>
        /// <param name="permission">The permission object to convert to PermissionCode</param>
        /// <returns>A string representation of the Permission object (aka permissionCode)</returns>
        public static string ToPermissionCode(PermissionUserToExecute permission) =>
            string.Format(PermissionFormat, permission.Service, permission.Module, permission.Action);

        /// <summary>
        /// Converts the string representation of a <see cref="PermissionUserToExecute"/> object
        /// to an actual object instance.
        /// </summary>
        /// <param name="permissionFormatted">Formatted permission representation</param>
        /// <returns></returns>
        public static PermissionUserToExecute ToPermissionObject(string permissionFormatted)
        {
            var parts = permissionFormatted.Split(PermissionSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
                throw new FormatException(SystemMessages.E_WRONG_PERMISSION_FORMAT);

            return new PermissionUserToExecute
            {
                Service = parts[0],
                Module = parts[1],
                Action = parts[2]
            };
        }
    }
}