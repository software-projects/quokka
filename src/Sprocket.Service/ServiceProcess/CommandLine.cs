using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NDesk.Options;
using Quokka.Diagnostics;

namespace Sprocket.Service.ServiceProcess
{
    public class CommandLine
    {
        public bool Install { get; private set; }
        public bool Uninstall { get; private set; }
        public bool ConsoleMode { get; private set; }
        public bool ShowHelp { get; private set; }
        public List<String> Errors { get; private set; }
        private readonly OptionSet _optionSet;

        public CommandLine(IEnumerable<string> args)
        {
            Errors = new List<string>();
            _optionSet = new OptionSet
                                {
                                    {"i|install", "Install service", v => Install = v != null},
                                    {"u|uninstall", "Uninstall service", v => Uninstall = v != null},
                                    {"c|console", "Run in console mode", v => ConsoleMode = v != null},
                                    {"?|help", "Show this usage text", v => ShowHelp = v != null}
                                };

            try
            {
                List<string> extras = _optionSet.Parse(args);
                if (extras.Count > 0)
                {
                    foreach (string extra in extras)
                    {
                       Errors.Add("Invalid command line argument: " + extra);
                    }
                }

                int count = 0;
                if (Install)
                {
                    count++;
                }
                if (Uninstall)
                {
                    count++;
                }
                if (ConsoleMode)
                {
                    count++;
                }
                if (count > 1)
                {
                    Errors.Add("Choose one only of --install, --uninstall, or --console");
                }
                if (count == 0 && !ShowHelp)
                {
                    Errors.Add("Choose one of --install, --uninstall or --console");
                }
            }
            catch (OptionException ex)
            {
                CreateErrorMessage(ex);
            }
            catch (OverflowException ex)
            {
                CreateErrorMessage(ex);
            }
            catch (FormatException ex)
            {
                CreateErrorMessage(ex);
            }
        }

        public bool Error
        {
            get { return Errors.Count > 0; }
        }

        public void WriteUsage(TextWriter writer)
        {
            Verify.ArgumentNotNull(writer, "writer");
            writer.WriteLine("usage: {0} [ options ]", ProgramName);
            writer.WriteLine("options:");
            _optionSet.WriteOptionDescriptions(writer);
        }

        public void WriteErrorMessage(TextWriter writer)
        {
            Verify.ArgumentNotNull(writer, "writer");
            foreach (var errorMessage in Errors)
            {
                writer.WriteLine(errorMessage);
            }
            WriteShortHelpMessage(writer);
        }

        public void WriteShortHelpMessage(TextWriter writer)
        {
            writer.WriteLine("Type '{0} --help' for command line usage.", ProgramName);
        }

        private void CreateErrorMessage(Exception ex)
        {
           Errors.Add("Invalid command line argument: " + ex.Message);
        }

        private static string ProgramName
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }
    }
}
