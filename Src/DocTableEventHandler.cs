using System;
using System.Linq;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace GitForP4
{
    internal class DocTableEventHandler : IVsRunningDocTableEvents3
    {
        private readonly DTE _dte;
        private readonly RunningDocumentTable _runningDocumentTable;
        
        public DocTableEventHandler(DTE dte, RunningDocumentTable runningDocumentTable)
        {
            _dte = dte;
            _runningDocumentTable = runningDocumentTable;
        }

        public int OnBeforeSave(uint docCookie)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Log("Saving document " + docCookie);
            Log("Docs " + _dte.Documents.Count);

            var Documents = _dte.Documents.Cast<Document>();
            var DocumentToFormat = CookieToDoc(docCookie, Documents);
            if (DocumentToFormat != null)
            {
                Log(DocumentToFormat.Path);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie) { return VSConstants.S_OK; }
        public int OnAfterFirstDocumentLock(uint DocCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) { return VSConstants.S_OK; }
        public int OnBeforeLastDocumentUnlock(uint DocCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) { return VSConstants.S_OK; }
        public int OnAfterAttributeChange(uint DocCookie, uint grfAttribs) { return VSConstants.S_OK; }
        public int OnBeforeDocumentWindowShow(uint DocCookie, int fFirstShow, IVsWindowFrame pFrame) { return VSConstants.S_OK; }
        public int OnAfterDocumentWindowHide(uint DocCookie, IVsWindowFrame pFrame) { return VSConstants.S_OK; }
        int IVsRunningDocTableEvents3.OnAfterAttributeChangeEx(uint DocCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) { return VSConstants.S_OK; }
        int IVsRunningDocTableEvents2.OnAfterAttributeChangeEx(uint DocCookie, uint grfAttribs, IVsHierarchy pHierOld, uint itemidOld, string pszMkDocumentOld, IVsHierarchy pHierNew, uint itemidNew, string pszMkDocumentNew) { return VSConstants.S_OK; }

        private void Log(string message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = _dte as DTE2;
            if (dte2 != null)
            {
                dte2.ToolWindows.OutputWindow.ActivePane.OutputString(message);
            }
        }

        private Document CookieToDoc(uint docCookie, IEnumerable<Document> documents)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (var Doc in documents)
            {
                if (Doc.FullName == _runningDocumentTable.GetDocumentInfo(docCookie).Moniker)
                {
                    return Doc;
                }
            }
            return null;
        }
    }
}