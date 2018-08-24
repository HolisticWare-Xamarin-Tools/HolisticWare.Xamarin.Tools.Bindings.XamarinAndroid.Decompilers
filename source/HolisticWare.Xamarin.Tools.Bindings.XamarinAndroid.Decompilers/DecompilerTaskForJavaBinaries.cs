using System;

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
                /*
                javap \
                    -classpath ../../../../externals/android/grpc-stub-1.14.0.jar \
                        $(jar -tf ../../../../externals/android/grpc-stub-1.14.0.jar | grep "class$" | sed s/\.class$//) \
                        >> grpc-stub-1.14.0.jar.class.java.txt
                */
                ProcessStart
                (
                    $@"javap",
                    $@"-classpath {JarBinaryAndroidArtifact} $(jar -tf {JarBinaryAndroidArtifact} | grep \"class$\" | sed s/\.class$//) {Options}"
                );
            }
            else
            {
                switch(JarBinaryDecompiler.ToLower())
                {
                    case "lib/procyon-decompiler-0.5.30.jar":
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
            p.StartInfo = pi;
            p.OutputDataReceived += OutputDataReceived;
            p.ErrorDataReceived += ErrorDataReceived;

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