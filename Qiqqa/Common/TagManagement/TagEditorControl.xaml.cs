using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Qiqqa.DocumentLibrary;
using Qiqqa.UtilisationTracking;
using Utilities;
using Utilities.GUI;

namespace Qiqqa.Common.TagManagement
{
    /// <summary>
    /// Interaction logic for TagEditorControl.xaml
    /// </summary>
    public partial class TagEditorControl : UserControl, IDisposable
    {
        internal Feature TagFeature_Add { get; set; }
        internal Feature TagFeature_Remove { get; set; }

        // TODO
        //
        // Warning CA1001  Implement IDisposable on 'TagEditorControl' because it creates members 
        // of the following IDisposable types: 'WeakDependencyPropertyChangeNotifier'. 
        // If 'TagEditorControl' has previously shipped, adding new members that implement IDisposable 
        // to this type is considered a breaking change to existing consumers.

        private WeakDependencyPropertyChangeNotifier wdpcn;

        public TagEditorControl()
        {
            InitializeComponent();

            ObjAddControl.OnNewTag += ObjAddControl_OnNewTag;

            // Register for notifications of changes to the COMPONENT's TagsBundle
            wdpcn = new WeakDependencyPropertyChangeNotifier(this, TagsBundleProperty);
            wdpcn.ValueChanged += OnTagsBundlePropertyChanged;
        }

        private void OnTagsBundlePropertyChanged(object sender, EventArgs e)
        {
            RebuildTagItems();
        }

        private void ObjAddControl_OnNewTag(string tag)
        {
            HashSet<string> tags = TagTools.ConvertTagBundleToTags(TagsBundle);
            tags.Add(tag);
            TagsBundle = TagTools.ConvertTagListToTagBundle(tags);

            // Invoke feature tracking if it was requested
            if (null != TagFeature_Add)
            {
                FeatureTrackingManager.Instance.UseFeature(TagFeature_Add);
            }
        }

        private void RebuildTagItems()
        {
            string tags_string = TagsBundle;
            List<string> tags = new List<string>(TagTools.ConvertTagBundleToTags(tags_string));
            tags.Sort();

            // Clear out the old tags
            List<TagEditorItemControl> old_tags = new List<TagEditorItemControl>(ObjTagsPanel.Children.OfType<TagEditorItemControl>());
            old_tags.ForEach(o => ObjTagsPanel.Children.Remove(o));

            foreach (string tag in tags)
            {
                ObjTagsPanel.Children.Add(new TagEditorItemControl(tag, OnTagRemoved));
            }

            TagManager.Instance.ProcessTags(tags);
        }

        public void OnTagRemoved(string tag)
        {
            // Invoke feature tracking if it was requested
            if (null != TagFeature_Remove)
            {
                FeatureTrackingManager.Instance.UseFeature(TagFeature_Remove);
            }

            HashSet<string> tags = TagTools.ConvertTagBundleToTags(TagsBundle);
            tags.Remove(tag);
            TagsBundle = TagTools.ConvertTagListToTagBundle(tags);
        }

        public Visibility TagsTitleVisibility
        {
            get => TxtTagTitle.Visibility;
            set => TxtTagTitle.Visibility = value;
        }

        public static DependencyProperty TagsBundleProperty = DependencyProperty.Register("TagsBundle", typeof(string), typeof(TagEditorControl), new PropertyMetadata());
        public string TagsBundle
        {
            get => (string)GetValue(TagsBundleProperty);
            set => SetValue(TagsBundleProperty, value);
        }

        #region --- IDisposable ------------------------------------------------------------------------

        ~TagEditorControl()
        {
            Logging.Debug("~TagEditorControl()");
            Dispose(false);
        }

        public void Dispose()
        {
            Logging.Debug("Disposing TagEditorControl");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private int dispose_count = 0;
        protected virtual void Dispose(bool disposing)
        {
            Logging.Debug("TagEditorControl::Dispose({0}) @{1}", disposing, dispose_count);

            // *Nobody* gets any updates from us anymore, so we can delete cached content etc. in peace. (https://github.com/jimmejardine/qiqqa-open-source/issues/121)
            WPFDoEvents.InvokeInUIThread(() =>
            {
                BindingOperations.ClearBinding(this, TagsBundleProperty);

                if (null != wdpcn)
                {
                    wdpcn.ValueChanged -= OnTagsBundlePropertyChanged;
                }
                // TagsBundle = null;  <-- forbidden to reset as that MAY trigger a dependency update! (https://github.com/jimmejardine/qiqqa-open-source/issues/121)

                ObjTagsPanel.Children.Clear();
            });

            // Get rid of managed resources
            wdpcn?.Dispose();

            ObjAddControl.OnNewTag -= ObjAddControl_OnNewTag;

            wdpcn = null;

            ++dispose_count;
        }

        #endregion
    }
}
