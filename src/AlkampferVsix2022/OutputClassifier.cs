using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;

namespace AlkampferVsix2022
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "OutputClassifier" classification type.
    /// </summary>
    internal class OutputClassifier : IClassifier
    {
        /// <summary>
        /// Classification type.
        /// </summary>
        private readonly IClassificationType classificationType;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal OutputClassifier(IClassificationTypeRegistryService registry)
        {
            this.classificationType = registry.GetClassificationType("OutputClassifier");
            _classificationTypeRegistry = registry;
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            ITextSnapshot snapshot = span.Snapshot;

            List<ClassificationSpan> spans = new List<ClassificationSpan>();

            if (snapshot.Length == 0)
                return spans;

            var text = span.GetText();

            if (text.StartsWith("System.Windows.Data Error", StringComparison.OrdinalIgnoreCase))
            {
                IClassificationType type = _classificationTypeRegistry.GetClassificationType("output.wpfbindingalert");
                spans.Add(new ClassificationSpan(span, type));
            }
            else if (text.IndexOf("error ", StringComparison.OrdinalIgnoreCase) >= 0 ||
                     text.IndexOf("error:", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //error followed by a space is the typical error of the build.
                IClassificationType type = _classificationTypeRegistry.GetClassificationType("output.alert");
                spans.Add(new ClassificationSpan(span, type));
            }
            else if (text.IndexOf("INFO:", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //error followed by a space is the typical error of the build.
                IClassificationType type = _classificationTypeRegistry.GetClassificationType("output.info");
                spans.Add(new ClassificationSpan(span, type));
            }

            return spans;
        }

        #endregion
    }
}
