﻿using System;

namespace HolisticWare.Xamarin.Tools.Bindings.XamarinAndroid.Decompilers
{
    public class DecompilerTaskForJavaBinaries : Microsoft.Build.Utilities.Task
    {
        [Microsoft.Build.Framework.Required]
        public string Executable
        {
            get;
            set;
        }

        [Microsoft.Build.Framework.Required]
        public string JarBinaryDecompiler
        {
            get;
            set;
        }

        [Microsoft.Build.Framework.Required]
        public string JarBinaryAndroidArtifact
        {
            get;
            set;
        }

        [Microsoft.Build.Framework.Required]
        public string Options
        {
            get;
            set;
        }

        public override bool Execute()
        {

            Log.LogMessage($"DecompilerTaskForJavaBinaries.Execute: ");
            Log.LogMessage($"            Executable               : {Executable}");
            Log.LogMessage($"            JarBinaryDecompiler      : {JarBinaryDecompiler}");
            Log.LogMessage($"            JarBinaryAndroidArtifact : {JarBinaryAndroidArtifact}");
            Log.LogMessage($"            Options                  : {Options}");


            if (Executable.ToLower().Equals("javap"))
            {
                filename_error = "holisticware-generated/decompilers/error-javap.log";
                filename_output = "holisticware-generated/decompilers/output-javap.log";
                /*
                javap \
                    -classpath ../../../../externals/android/grpc-stub-1.14.0.jar \
                        $(jar -tf ../../../../externals/android/grpc-stub-1.14.0.jar | grep "class$" | sed s/\.class$//) \
                        >> grpc-stub-1.14.0.jar.class.java.txt
                */
                ProcessStart
                (
                    $@"javap",
                    $"-classpath {JarBinaryAndroidArtifact} $(jar -tf {JarBinaryAndroidArtifact} | grep \"class$\" | sed s/\\.class$//) {Options}"
                );
            }
            else
            {
                switch(JarBinaryDecompiler.ToLower())
                {
                    case "lib/procyon-decompiler-0.5.30.jar":
                        filename_error = "holisticware-generated/decompilers/error-procyon.log";
                        filename_output = "holisticware-generated/decompilers/output-procyon.log";
                        /*
                        java \
                            -jar ./procyon-decompiler-0.5.30.jar \
                            -jar ../../../../externals/android/grpc-stub-1.14.0.jar > 

                        */
                        ProcessStart
                        (
                            $@"java",
                            $@"-jar {JarBinaryDecompiler} -jar {JarBinaryAndroidArtifact} {Options} "
                        );
                        break;
                    case "lib/cfr_0_132.jar":
                        filename_error = "holisticware-generated/decompilers/error-cfr.log";
                        filename_output = "holisticware-generated/decompilers/output-cfr.log";
                        /*
                        java \
                            -jar ./cfr_0_132.jar \
                            -jar ../../../../externals/android/grpc-protobuf-lite-1.14.0.jar
                        */
                        ProcessStart
                        (
                            $@"java",
                            $@"-jar {JarBinaryDecompiler} -jar {JarBinaryAndroidArtifact} {Options} "
                        );
                        break;
                    case "lib/bytecode-viewer-2.9.11.jar":
                        filename_error = "holisticware-generated/decompilers/error-bytecode-viewer.log";
                        filename_output = "holisticware-generated/decompilers/output-bytecode-viewer.log";
                        ProcessStart
                        (
                            $@"java",
                            $@"-jar {Executable} -jar {JarBinaryDecompiler} "
                        );
                        break;
                    default:
                        throw new NotSupportedException($"Unrecognized Java DEcompiler {JarBinaryDecompiler}");
                }

            }

            // enforcing proper correlation between Log errors and build results (success and/or failures)
            return !Log.HasLoggedErrors;
        }

        string filename_output;
        string filename_error;

        protected void ProcessStart(string executable, string arguments )
        {
            Log.LogMessage($"                   ProcessStart executable:");
            Log.LogMessage($@"                   {executable} \");
            Log.LogMessage($@"                   {arguments} \");
            System.Diagnostics.ProcessStartInfo pi = null;
            pi = new System.Diagnostics.ProcessStartInfo(executable, arguments);
            pi.UseShellExecute = false;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;


            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.EnableRaisingEvents = true;
            p.StartInfo = pi;
            p.OutputDataReceived += OutputDataReceived;
            p.ErrorDataReceived += ErrorDataReceived;
            p.Start();

            using (System.IO.StreamReader so = p.StandardOutput)
            using (System.IO.StreamReader se = p.StandardError)
            {
                string output = so.ReadToEnd();
                string error = se.ReadToEnd();
                p.WaitForExit(20000);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename_output))
                {
                    file.WriteLine(output);
                    file.Flush();
                    file.Close();
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename_error))
                {
                    file.WriteLine(error);
                    file.Flush();
                    file.Close();
                }
            }

            return;
        }

        protected void OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Log.LogMessage($"decompiler stdout: {e.Data}");

            return;
        }

        protected void ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            Log.LogMessage($"decompiler stderr: {e.Data}");

            return;
        }

    }
}