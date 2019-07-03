using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Qiqqa.DocumentLibrary;
using Qiqqa.UtilisationTracking;
using Utilities.GUI;

namespace Qiqqa.Common.TagManagement
{
    /// <summary>
    /// Interaction logic for TagEditorControl.xaml
    /// </summary>
    public partial class TagEditorControl : UserControl
    {
        internal Feature TagFeature_Add { get; set; }
        internal Feature TagFeature_Remove { get; set; }

        WeakDependencyPropertyChangeNotifier wdpcn;

        public TagEditorControl()
        {
            InitializeComponent();

            ObjAddControl.OnNewTag += ObjAddControl_OnNewTag;

            // Register for notifications of changes to the COMPONENT's TagsBundle
            wdpcn = new WeakDependencyPropertyChangeNotifier(this, TagsBundleProperty);
            wdpcn.ValueChanged += OnTagsBundlePropertyChanged;
        }

        void OnTagsBundlePropertyChanged(object sender, EventArgs e)
        {
            RebuildTagItems();
        }

        void ObjAddControl_OnNewTag(string tag)
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
            set
            {
                TxtTagTitle.Visibility = value;
            }
        }

        public static DependencyProperty TagsBundleProperty = DependencyProperty.Register("TagsBundle", typeof(string), typeof(TagEditorControl), new PropertyMetadata());
        public string TagsBundle
        {
            get { return (string)GetValue(TagsBundleProperty); }
            set { SetValue(TagsBundleProperty, value); }
        }
    }
}
