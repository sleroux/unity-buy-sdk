namespace Shopify.Unity.SDK.Editor {
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Text;
    using Shopify.Unity.SDK;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// A Query Loader built to load queries in editor scripts, when the normal game loop is not available.
    /// </summary>
    public class UnityEditorLoader : BaseLoader {
        public UnityEditorLoader(string domain, string accessToken) : base(domain, accessToken) {
            if (domain.Trim().Length == 0) {
                throw new ArgumentException("Domain is invalid. Make sure that it is not empty/blank.");
            }

            EditorApplication.update += Update;
        }

        ~UnityEditorLoader() {
            EditorApplication.update -= Update;
        }

        #region Interface

        public override void Load(string query, LoaderResponseHandler callback) {
            var body = Encoding.ASCII.GetBytes(query);
            var w = new WWW(Url, body, _headers);
            _activeRequests.Add(ProcessRequest(w, callback));
        }

        public override string SDKVariantName() {
            return "unity";
        }

        public override void SetHeader(string key, string value) {
            _headers.Add(key, value);
        }

        #endregion

        #region Implementation

        private List<IEnumerator> _activeRequests = new List<IEnumerator>();

        private Dictionary<string, string> _headers = new Dictionary<string, string>();

        private string Url {
            get {
                return "https://" + Domain + "/api/2020-01/graphql.json";
            }
        }

        private IEnumerator ProcessRequest(WWW w, LoaderResponseHandler callback) {
            // We have to use this enumerator approach because the www class inherits from CustomYieldInstruction,
            // which isn't well supported in the editor.

            while (!w.isDone) {
                yield return null;
            }

            if (!string.IsNullOrEmpty(w.error)) {
                callback(null, w.error);
            } else {
                callback(w.text, null);
            }

            w.Dispose();
        }

        private void Update() {
            foreach (var request in new List<IEnumerator>(_activeRequests)) {
                if (!request.MoveNext()) {
                    _activeRequests.Remove(request);
                }
            }
        }

        #endregion
    }

    public class UnityEditorLoaderProvider : ILoaderProvider {
        BaseLoader ILoaderProvider.GetLoader(string accessToken, string domain) {
            return new UnityEditorLoader(domain, accessToken);
        }
    }
}
