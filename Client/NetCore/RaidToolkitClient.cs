using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Raid.Toolkit.Common;
using Raid.Toolkit.Common.API;
using Raid.Toolkit.DataModel;

namespace Raid.Client
{
    public class RaidToolkitClient
    {
        private readonly ApiClientBase Client;
		public IAccountApi AccountApi => MakeApi<AccountApi>();
        public IStaticDataApi StaticDataApi => MakeApi<StaticDataApi>();
        public IRealtimeApi RealtimeApi => MakeApi<RealtimeApi>();

		public RaidToolkitClient(Uri endpointUri = null)
		{
            Client = new SocketApiClient(endpointUri);
		}

		public T MakeApi<T>()
		{
			return (T)Activator.CreateInstance(typeof(T), Client) ?? throw new InvalidOperationException($"Could not construct object of type {typeof(T).FullName}");
		}

		public static async Task EnsureInstalled()
        {
            if (!RegistrySettings.IsInstalled)
            {
                using var form = new Form { TopMost = true };
                var response = MessageBox.Show(
                    form,
                    "Raid Toolkit is required to be installed to access game data, would you like to download and install it now?",
                    "Installation required",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                    );
                if (response != DialogResult.Yes)
                {
                    throw new NotSupportedException("Raid Toolkit must be installed");
                }
                try
                {
                    await InstallRTK();
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(form, $"An error ocurred\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static async Task InstallRTK()
        {
            GitHub.Updater updater = new();
            GitHub.Schema.Release release = await updater.GetLatestRelease() 
                ?? throw new FileNotFoundException("Could not find the latest release");

			string tempFile = Path.Combine(Path.GetTempPath(), "RaidToolkitSetup.exe");
            using (var stream = await updater.DownloadSetup(release, null))
            {
                using Stream newFile = File.Create(tempFile);
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(newFile);
            }
            Process proc = Process.Start(tempFile);
            await proc.WaitForExitAsync();
        }
    }
}
