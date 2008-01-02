using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NUnit.UiKit;

namespace NUnit.Gui.SettingsPages
{
	public class TreeSettingsPage : NUnit.UiKit.SettingsPage
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox initialDisplayComboBox;
		private System.Windows.Forms.CheckBox clearResultsCheckBox;
		private System.Windows.Forms.CheckBox saveVisualStateCheckBox;
		private System.Windows.Forms.CheckBox showCheckBoxesCheckBox;
		private System.Windows.Forms.HelpProvider helpProvider1;
		private System.ComponentModel.IContainer components = null;

		public TreeSettingsPage(string key) : base(key)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.initialDisplayComboBox = new System.Windows.Forms.ComboBox();
			this.clearResultsCheckBox = new System.Windows.Forms.CheckBox();
			this.saveVisualStateCheckBox = new System.Windows.Forms.CheckBox();
			this.showCheckBoxesCheckBox = new System.Windows.Forms.CheckBox();
			this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Location = new System.Drawing.Point(72, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(376, 8);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Tree View";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 24);
			this.label2.TabIndex = 32;
			this.label2.Text = "Initial display on load:";
			// 
			// initialDisplayComboBox
			// 
			this.initialDisplayComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.helpProvider1.SetHelpString(this.initialDisplayComboBox, "Selects the initial display style of the tree when an assembly is loaded");
			this.initialDisplayComboBox.ItemHeight = 16;
			this.initialDisplayComboBox.Items.AddRange(new object[] {
																		"Auto",
																		"Expand",
																		"Collapse",
																		"HideTests"});
			this.initialDisplayComboBox.Location = new System.Drawing.Point(184, 24);
			this.initialDisplayComboBox.Name = "initialDisplayComboBox";
			this.helpProvider1.SetShowHelp(this.initialDisplayComboBox, true);
			this.initialDisplayComboBox.Size = new System.Drawing.Size(87, 24);
			this.initialDisplayComboBox.TabIndex = 33;
			// 
			// clearResultsCheckBox
			// 
			this.helpProvider1.SetHelpString(this.clearResultsCheckBox, "If checked, any prior results are cleared when reloading");
			this.clearResultsCheckBox.Location = new System.Drawing.Point(32, 56);
			this.clearResultsCheckBox.Name = "clearResultsCheckBox";
			this.helpProvider1.SetShowHelp(this.clearResultsCheckBox, true);
			this.clearResultsCheckBox.Size = new System.Drawing.Size(232, 24);
			this.clearResultsCheckBox.TabIndex = 34;
			this.clearResultsCheckBox.Text = "Clear results when reloading.";
			// 
			// saveVisualStateCheckBox
			// 
			this.helpProvider1.SetHelpString(this.saveVisualStateCheckBox, "If checked, the visual state of the project is saved on exit. This includes selec" +
				"ted tests, categories and the state of the tree itself.");
			this.saveVisualStateCheckBox.Location = new System.Drawing.Point(32, 88);
			this.saveVisualStateCheckBox.Name = "saveVisualStateCheckBox";
			this.helpProvider1.SetShowHelp(this.saveVisualStateCheckBox, true);
			this.saveVisualStateCheckBox.Size = new System.Drawing.Size(248, 24);
			this.saveVisualStateCheckBox.TabIndex = 35;
			this.saveVisualStateCheckBox.Text = "Save Visual State of each project";
			// 
			// showCheckBoxesCheckBox
			// 
			this.helpProvider1.SetHelpString(this.showCheckBoxesCheckBox, "If selected, the tree displays checkboxes for use in selecting multiple tests.");
			this.showCheckBoxesCheckBox.Location = new System.Drawing.Point(32, 120);
			this.showCheckBoxesCheckBox.Name = "showCheckBoxesCheckBox";
			this.helpProvider1.SetShowHelp(this.showCheckBoxesCheckBox, true);
			this.showCheckBoxesCheckBox.Size = new System.Drawing.Size(264, 24);
			this.showCheckBoxesCheckBox.TabIndex = 36;
			this.showCheckBoxesCheckBox.Text = "Show CheckBoxes";
			// 
			// TreeSettingsPage
			// 
			this.Controls.Add(this.showCheckBoxesCheckBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.initialDisplayComboBox);
			this.Controls.Add(this.clearResultsCheckBox);
			this.Controls.Add(this.saveVisualStateCheckBox);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "TreeSettingsPage";
			this.ResumeLayout(false);

		}
		#endregion

		public override void LoadSettings()
		{
			initialDisplayComboBox.SelectedIndex = (int)(TestSuiteTreeView.DisplayStyle)settings.GetSetting( "Gui.TestTree.InitialTreeDisplay", TestSuiteTreeView.DisplayStyle.Auto );
			clearResultsCheckBox.Checked = settings.GetSetting( "Options.TestLoader.ClearResultsOnReload", true );
			saveVisualStateCheckBox.Checked = settings.GetSetting( "Gui.TestTree.SaveVisualState", true );
			showCheckBoxesCheckBox.Checked = settings.GetSetting( "Options.ShowCheckBoxes", false );
		}

		public override void ApplySettings()
		{
			settings.SaveSetting( "Gui.TestTree.InitialTreeDisplay", (TestSuiteTreeView.DisplayStyle)initialDisplayComboBox.SelectedIndex );
			settings.SaveSetting( "Options.TestLoader.ClearResultsOnReload", clearResultsCheckBox.Checked );
			settings.SaveSetting( "Gui.TestTree.SaveVisualState", saveVisualStateCheckBox.Checked );
			settings.SaveSetting( "Options.ShowCheckBoxes", showCheckBoxesCheckBox.Checked );
		}
	}
}

