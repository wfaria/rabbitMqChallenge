namespace JsonHelper
{
    using System;
    using JsonHelper.Model;

    class Program
    {
        static void Main(string[] args)
        {
            var testString = @"
                {
                'AdditionalData' : {},
                'ApplicationId' : 'f24dbb31-a065-4fa7-b6f8-19cddac7d7c5',
                'MachineName' : 'MachineName',
                'Message' : 'A new user was created.',
                'NativeProcessId' : '28552',
                'NativeThreadId' : '10488',
                'OSFullName' : 'Microsoft Windows NT 6.2.9200.0',
                'ProcessName' : 'MyProcess.exe',
                'ProcessPath' : 'C:\\Applications\\MyProcess\\MyProcess.exe',
                'ProductCompany' : 'MyCompany',
                'ProductName' : 'MyProduct',
                'ProductVersion' : '1.0.0',
                'Severity' : 'Info',
                'Tags' : [],
                'Timestamp' : '2016-08-24T18:30:32.2387069+00:00',
                'TypeName' : 'LogEntry'
                }
            ";

            var dyn = new LogEntry().Deserialize(testString);
            Console.WriteLine(dyn.Severity);

            var listText = string.Format("[{0}, {1}]", testString, testString);
            var dynList = new LogEntryList().Deserialize(listText);
            Console.WriteLine(dynList[1].ProductCompany);
        }
    }
}
