using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DiagnosticAdapter;

namespace Storage.EFCore
{
    /// <summary> Интерцептор выставляет дефолтный LC_LOCALE ru_RU.UTF-8 </summary>
    public class CommandListener
    {
        [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuting")]
        public void OnCommandExecuting(DbCommand command, DbCommandMethod executeMethod, Guid commandId, Guid connectionId, bool async, DateTimeOffset startTime)
        {
//            if (command.CommandText.Contains("CREATE DATABASE"))
//            {
//                command.CommandText = command.CommandText
//                    .Replace(Environment.NewLine, string.Empty)
//                    .Replace(";", string.Empty);
//
//                command.CommandText +=
//                    $@" ENCODING 'UTF-8' 
//                    LC_COLLATE 'ru_RU.UTF-8'
//                    LC_CTYPE 'ru_RU.UTF-8'
//                    TEMPLATE template0; ";
//            }
        }

        [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")]
        public void OnCommandExecuted(object result, bool async) { }

        [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Command.CommandError")]
        public void OnCommandError(Exception exception, bool async) { }
    }
}