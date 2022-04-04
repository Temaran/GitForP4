using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using EnvDTE;

namespace GitForP4
{
	[Microsoft.VisualStudio.AsyncPackageHelpers.AsyncPackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, Microsoft.VisualStudio.AsyncPackageHelpers.PackageAutoLoadFlags.BackgroundLoad)]
	[Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, Microsoft.VisualStudio.AsyncPackageHelpers.PackageAutoLoadFlags.BackgroundLoad)]
	[Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, Microsoft.VisualStudio.AsyncPackageHelpers.PackageAutoLoadFlags.BackgroundLoad)]
	[Microsoft.VisualStudio.AsyncPackageHelpers.ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, Microsoft.VisualStudio.AsyncPackageHelpers.PackageAutoLoadFlags.BackgroundLoad)]
	[Guid("035efc93-c6ac-4591-a200-b58286bde51f")]
	public sealed class GitForP4Package : AsyncPackage
	{
		private DocTableEventHandler _eventHandler;

		protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await base.InitializeAsync(cancellationToken, progress);

			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			DTE dte = (DTE)await GetServiceAsync(typeof(DTE));

			var runningDocumentTable = new RunningDocumentTable(this);
			_eventHandler = new DocTableEventHandler(dte, runningDocumentTable);
			runningDocumentTable.Advise(_eventHandler);
		}
	}
}
