#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::IO;

using namespace System::Runtime::InteropServices;

using namespace ZedGraph;
using namespace StopWatch;

#define	MAX_GRAPH_LISTS		255
#define	COLOR_COUNT			10

namespace CHRInterface {

	/// <summary>
	/// Summary for DataGraphDialog
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class DataGraphDialog : public System::Windows::Forms::Form
	{
	public:
		DataGraphDialog(void)
		{
			InitializeComponent();

			graphTime = gcnew Stopwatch;
			
			dataGraphList = gcnew cli::array<RollingPointPairList^>(MAX_GRAPH_LISTS);
			this->dataListCount = 0;

			loggingEnabled = false;

			dataItemIndexes = gcnew cli::array<UInt32,1>(MAX_GRAPH_LISTS);
			
			autoSetAxes = true;
			this->dataHistorySize = 100;
			title = L"";
			xLabel = L"Time (s)";
			yLabel = L"Sensor Output";
			yMin = 0;
			yMax = 0;
			
			timer1->Interval = 50;
			time = 0;

			colors = gcnew cli::array<Color>(COLOR_COUNT);
			colors[0] = Color::Blue;
			colors[1] = Color::Red;
			colors[2] = Color::Green;
			colors[3] = Color::Brown;
			colors[4] = Color::DarkBlue;
			colors[5] = Color::DarkRed;
			colors[6] = Color::DarkGreen;
			colors[7] = Color::Cyan;
			colors[8] = Color::DarkKhaki;
			colors[9] = Color::Aquamarine;

			PopulateGraphSettings();
		}

	public:
		void SetFirwarmwareReference( FirmwareDefinition^ firmware )
		{
			this->firmware = firmware;

			// Clear any existing nodes in data box
			dataItemTreeView->Nodes->Clear();

			this->dataItemsCopy = gcnew cli::array<FirmwareItem^>(this->firmware->GetDataItemCount());

			// Add nodes to data box
			int current_parent_index = 0;
			for( Int32 i = 0; i < this->dataItemsCopy->Length; i++ )
			{
				this->dataItemsCopy[i] = this->firmware->GetDataItem(i)->Duplicate();

				if( this->dataItemsCopy[i]->GetParentIndex() == i )
				{
					dataItemTreeView->Nodes->Add(this->dataItemsCopy[i]);
					current_parent_index = i;
				}
				else
				{
					dataItemsCopy[current_parent_index]->Nodes->Add( this->dataItemsCopy[i] );
				}
			}

			InitializeGraph();
			graphTime->Start();
			timer1->Start();

		}

	private:

		void PopulateGraphSettings()
		{
			this->checkBox_autoSetRange->Checked = this->autoSetAxes;
			this->textBox_historySize->Text = this->dataHistorySize.ToString();
			this->textBox_title->Text = this->title;
			this->textBox_xlabel->Text = this->xLabel;
			this->textBox_ylabel->Text = this->yLabel;
			this->textBox_yMax->Text = this->yMax.ToString();
			this->textBox_yMin->Text = this->yMin.ToString();						
		}

		void RetrieveGraphSettingsFromDialog()
		{
			this->title = this->textBox_title->Text;
			this->xLabel = this->textBox_xlabel->Text;
			this->yLabel = this->textBox_ylabel->Text;
			try
			{
				this->yMax = double::Parse(this->textBox_yMax->Text);
			}
			catch( Exception^ /*e*/ )
			{
				this->textBox_yMax->Text = this->yMax.ToString();
			}
			try
			{
				this->yMin = double::Parse(this->textBox_yMin->Text);
			}
			catch( Exception^ /*e*/ )
			{
				this->textBox_yMin->Text = this->yMin.ToString();
			}
			this->autoSetAxes = this->checkBox_autoSetRange->Checked;
			try
			{
				this->dataHistorySize = int::Parse(this->textBox_historySize->Text);
			}
			catch( Exception^ /*e*/ )
			{
				this->textBox_historySize->Text = this->dataHistorySize.ToString();
			}
		}

		void ClearGraph()
		{
			// Remove curves from graph pane
			this->graphControl->GraphPane->CurveList->Clear();

			this->dataListCount = 0;
		}


		void InitializeGraph()
		{
			this->graphControl->GraphPane->Title->Text = this->title;
			this->graphControl->GraphPane->XAxis->Title->Text = this->xLabel;
			this->graphControl->GraphPane->YAxis->Title->Text = this->yLabel;			

			// If autoSetRange isn't checked
			if( !this->autoSetAxes )
			{
				this->graphControl->GraphPane->YAxis->Scale->Max = this->yMax;
				this->graphControl->GraphPane->YAxis->Scale->Min = this->yMin;
			}
			else
			{
				// TODO: figure out how to make the graph auto scale and add that here
			}

			// Iterate through the data item list and find items that are selected.  For each one, add a new list to the graph pane
			for( int i = 0; i < dataItemsCopy->Length; i++ )
			{
				// Ignore parent items that are checked
				if( dataItemsCopy[i]->GetParentIndex() != i )
				{
					// Check to see if the current item is checked
					if( dataItemsCopy[i]->Checked )
					{
						// Item is checked.  Add new list view to graph
						dataGraphList[dataListCount] = gcnew RollingPointPairList(this->dataHistorySize);

						this->dataItemIndexes[dataListCount] = i;

						Color next_color = colors[dataListCount % COLOR_COUNT];
						this->graphControl->GraphPane->AddCurve(dataItemsCopy[i]->Text, dataGraphList[dataListCount], next_color, SymbolType::None);
						dataListCount++;
					}
				}
			}

			RefreshGraph();
		}

		void RefreshGraph()
        {
            graphControl->AxisChange();
            graphControl->Invalidate();
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~DataGraphDialog()
		{
			if (components)
			{
				delete components;
			}
		}
	private: ZedGraph::ZedGraphControl^  graphControl;
	private: System::Windows::Forms::TabControl^  tabControl;

	private: cli::array<RollingPointPairList^>^ dataGraphList;
	private: UInt32 dataListCount;

	private: System::Windows::Forms::TabPage^  tabPage1;
	private: System::Windows::Forms::TabPage^  tabPage2;
	private: System::Windows::Forms::TabPage^  tabPage3;
	private: System::Windows::Forms::TreeView^  dataItemTreeView;
	private: System::Windows::Forms::TextBox^  textBox_title;
	private: FirmwareDefinition^ firmware;
	private: cli::array<FirmwareItem^>^ dataItemsCopy;
	private: cli::array<UInt32,1>^ dataItemIndexes;

	private: cli::array<Color>^ colors;

	private: Stopwatch^ graphTime;
	
	private: System::IO::StreamWriter^ logFile;
	private: bool loggingEnabled;

	private: bool autoSetAxes;
	private: String^ title;
	private: String^ xLabel;
	private: String^ yLabel;
	private: int dataHistorySize;
	private: double yMin;
	private: double yMax;
	private: double time;


	private: System::Windows::Forms::Label^  staticText1;
	private: System::Windows::Forms::TextBox^  textBox_xlabel;

	private: System::Windows::Forms::Label^  label1;
	private: System::Windows::Forms::CheckBox^  checkBox_autoSetRange;

	private: System::Windows::Forms::TextBox^  textBox_ylabel;

	private: System::Windows::Forms::Label^  label2;
	private: System::Windows::Forms::Label^  label4;
	private: System::Windows::Forms::Label^  label3;
	private: System::Windows::Forms::TextBox^  textBox_yMax;

	private: System::Windows::Forms::TextBox^  textBox_yMin;
	private: System::Windows::Forms::Button^  button_applyChanges;


	private: System::Windows::Forms::GroupBox^  groupBox1;
	private: System::Windows::Forms::GroupBox^  groupBox2;
	private: System::Windows::Forms::TextBox^  textBox_historySize;

	private: System::Windows::Forms::Label^  label5;
	private: System::Windows::Forms::Timer^  timer1;
private: System::Windows::Forms::TabPage^  tabPage4;
private: System::Windows::Forms::TextBox^  logFileTextBox;

private: System::Windows::Forms::SaveFileDialog^  saveFileDialog1;
private: System::Windows::Forms::Button^  stopLoggingButton;
private: System::Windows::Forms::Button^  startLoggingButton;
private: System::Windows::Forms::Button^  logFileBrowseButton;
	protected: 
	private: System::ComponentModel::IContainer^  components;

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>


#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(DataGraphDialog::typeid));
			this->graphControl = (gcnew ZedGraph::ZedGraphControl());
			this->tabControl = (gcnew System::Windows::Forms::TabControl());
			this->tabPage1 = (gcnew System::Windows::Forms::TabPage());
			this->tabPage2 = (gcnew System::Windows::Forms::TabPage());
			this->dataItemTreeView = (gcnew System::Windows::Forms::TreeView());
			this->tabPage3 = (gcnew System::Windows::Forms::TabPage());
			this->textBox_historySize = (gcnew System::Windows::Forms::TextBox());
			this->label5 = (gcnew System::Windows::Forms::Label());
			this->button_applyChanges = (gcnew System::Windows::Forms::Button());
			this->groupBox1 = (gcnew System::Windows::Forms::GroupBox());
			this->textBox_ylabel = (gcnew System::Windows::Forms::TextBox());
			this->textBox_xlabel = (gcnew System::Windows::Forms::TextBox());
			this->textBox_title = (gcnew System::Windows::Forms::TextBox());
			this->staticText1 = (gcnew System::Windows::Forms::Label());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->groupBox2 = (gcnew System::Windows::Forms::GroupBox());
			this->checkBox_autoSetRange = (gcnew System::Windows::Forms::CheckBox());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->label4 = (gcnew System::Windows::Forms::Label());
			this->textBox_yMax = (gcnew System::Windows::Forms::TextBox());
			this->textBox_yMin = (gcnew System::Windows::Forms::TextBox());
			this->tabPage4 = (gcnew System::Windows::Forms::TabPage());
			this->stopLoggingButton = (gcnew System::Windows::Forms::Button());
			this->startLoggingButton = (gcnew System::Windows::Forms::Button());
			this->logFileBrowseButton = (gcnew System::Windows::Forms::Button());
			this->logFileTextBox = (gcnew System::Windows::Forms::TextBox());
			this->timer1 = (gcnew System::Windows::Forms::Timer(this->components));
			this->saveFileDialog1 = (gcnew System::Windows::Forms::SaveFileDialog());
			this->tabControl->SuspendLayout();
			this->tabPage1->SuspendLayout();
			this->tabPage2->SuspendLayout();
			this->tabPage3->SuspendLayout();
			this->groupBox1->SuspendLayout();
			this->groupBox2->SuspendLayout();
			this->tabPage4->SuspendLayout();
			this->SuspendLayout();
			// 
			// graphControl
			// 
			this->graphControl->Location = System::Drawing::Point(-4, 0);
			this->graphControl->Name = L"graphControl";
			this->graphControl->ScrollGrace = 0;
			this->graphControl->ScrollMaxX = 0;
			this->graphControl->ScrollMaxY = 0;
			this->graphControl->ScrollMaxY2 = 0;
			this->graphControl->ScrollMinX = 0;
			this->graphControl->ScrollMinY = 0;
			this->graphControl->ScrollMinY2 = 0;
			this->graphControl->Size = System::Drawing::Size(354, 183);
			this->graphControl->TabIndex = 0;
			// 
			// tabControl
			// 
			this->tabControl->Controls->Add(this->tabPage1);
			this->tabControl->Controls->Add(this->tabPage2);
			this->tabControl->Controls->Add(this->tabPage3);
			this->tabControl->Controls->Add(this->tabPage4);
			this->tabControl->Location = System::Drawing::Point(0, 0);
			this->tabControl->Name = L"tabControl";
			this->tabControl->SelectedIndex = 0;
			this->tabControl->Size = System::Drawing::Size(354, 204);
			this->tabControl->TabIndex = 1;
			this->tabControl->Resize += gcnew System::EventHandler(this, &DataGraphDialog::tabControl_Resize);
			// 
			// tabPage1
			// 
			this->tabPage1->Controls->Add(this->graphControl);
			this->tabPage1->Location = System::Drawing::Point(4, 22);
			this->tabPage1->Name = L"tabPage1";
			this->tabPage1->Padding = System::Windows::Forms::Padding(3);
			this->tabPage1->Size = System::Drawing::Size(346, 178);
			this->tabPage1->TabIndex = 0;
			this->tabPage1->Text = L"Graph";
			this->tabPage1->UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this->tabPage2->Controls->Add(this->dataItemTreeView);
			this->tabPage2->Location = System::Drawing::Point(4, 22);
			this->tabPage2->Name = L"tabPage2";
			this->tabPage2->Padding = System::Windows::Forms::Padding(3);
			this->tabPage2->Size = System::Drawing::Size(346, 178);
			this->tabPage2->TabIndex = 1;
			this->tabPage2->Text = L"Data";
			this->tabPage2->UseVisualStyleBackColor = true;
			// 
			// dataItemTreeView
			// 
			this->dataItemTreeView->CheckBoxes = true;
			this->dataItemTreeView->Location = System::Drawing::Point(0, 0);
			this->dataItemTreeView->Name = L"dataItemTreeView";
			this->dataItemTreeView->Size = System::Drawing::Size(350, 182);
			this->dataItemTreeView->TabIndex = 0;
			this->dataItemTreeView->AfterCheck += gcnew System::Windows::Forms::TreeViewEventHandler(this, &DataGraphDialog::dataItemTreeView_AfterCheck);
			// 
			// tabPage3
			// 
			this->tabPage3->Controls->Add(this->textBox_historySize);
			this->tabPage3->Controls->Add(this->label5);
			this->tabPage3->Controls->Add(this->button_applyChanges);
			this->tabPage3->Controls->Add(this->groupBox1);
			this->tabPage3->Controls->Add(this->groupBox2);
			this->tabPage3->Location = System::Drawing::Point(4, 22);
			this->tabPage3->Name = L"tabPage3";
			this->tabPage3->Padding = System::Windows::Forms::Padding(3);
			this->tabPage3->Size = System::Drawing::Size(346, 178);
			this->tabPage3->TabIndex = 2;
			this->tabPage3->Text = L"Settings";
			this->tabPage3->UseVisualStyleBackColor = true;
			this->tabPage3->Click += gcnew System::EventHandler(this, &DataGraphDialog::tabPage3_Click);
			// 
			// textBox_historySize
			// 
			this->textBox_historySize->Location = System::Drawing::Point(25, 139);
			this->textBox_historySize->Name = L"textBox_historySize";
			this->textBox_historySize->Size = System::Drawing::Size(100, 20);
			this->textBox_historySize->TabIndex = 6;
			this->textBox_historySize->TextChanged += gcnew System::EventHandler(this, &DataGraphDialog::textBox_historySize_TextChanged);
			// 
			// label5
			// 
			this->label5->AutoSize = true;
			this->label5->Location = System::Drawing::Point(22, 123);
			this->label5->Name = L"label5";
			this->label5->Size = System::Drawing::Size(123, 13);
			this->label5->TabIndex = 14;
			this->label5->Text = L"History Size (data points)";
			// 
			// button_applyChanges
			// 
			this->button_applyChanges->Enabled = false;
			this->button_applyChanges->Location = System::Drawing::Point(235, 136);
			this->button_applyChanges->Name = L"button_applyChanges";
			this->button_applyChanges->Size = System::Drawing::Size(100, 23);
			this->button_applyChanges->TabIndex = 11;
			this->button_applyChanges->Text = L"Apply Changes";
			this->button_applyChanges->UseVisualStyleBackColor = true;
			this->button_applyChanges->Click += gcnew System::EventHandler(this, &DataGraphDialog::button_applyChanges_Click);
			// 
			// groupBox1
			// 
			this->groupBox1->Controls->Add(this->textBox_ylabel);
			this->groupBox1->Controls->Add(this->textBox_xlabel);
			this->groupBox1->Controls->Add(this->textBox_title);
			this->groupBox1->Controls->Add(this->staticText1);
			this->groupBox1->Controls->Add(this->label1);
			this->groupBox1->Controls->Add(this->label2);
			this->groupBox1->Location = System::Drawing::Point(8, 6);
			this->groupBox1->Name = L"groupBox1";
			this->groupBox1->Size = System::Drawing::Size(201, 107);
			this->groupBox1->TabIndex = 12;
			this->groupBox1->TabStop = false;
			this->groupBox1->Text = L"Graph Text";
			// 
			// textBox_ylabel
			// 
			this->textBox_ylabel->Location = System::Drawing::Point(63, 71);
			this->textBox_ylabel->Name = L"textBox_ylabel";
			this->textBox_ylabel->Size = System::Drawing::Size(100, 20);
			this->textBox_ylabel->TabIndex = 5;
			this->textBox_ylabel->TextChanged += gcnew System::EventHandler(this, &DataGraphDialog::textBox_ylabel_TextChanged);
			// 
			// textBox_xlabel
			// 
			this->textBox_xlabel->Location = System::Drawing::Point(63, 45);
			this->textBox_xlabel->Name = L"textBox_xlabel";
			this->textBox_xlabel->Size = System::Drawing::Size(100, 20);
			this->textBox_xlabel->TabIndex = 3;
			this->textBox_xlabel->TextChanged += gcnew System::EventHandler(this, &DataGraphDialog::textBox_xlabel_TextChanged);
			// 
			// textBox_title
			// 
			this->textBox_title->Location = System::Drawing::Point(63, 18);
			this->textBox_title->Name = L"textBox_title";
			this->textBox_title->Size = System::Drawing::Size(100, 20);
			this->textBox_title->TabIndex = 1;
			this->textBox_title->TextChanged += gcnew System::EventHandler(this, &DataGraphDialog::textBox_title_TextChanged);
			// 
			// staticText1
			// 
			this->staticText1->AutoSize = true;
			this->staticText1->Location = System::Drawing::Point(30, 22);
			this->staticText1->Name = L"staticText1";
			this->staticText1->Size = System::Drawing::Size(27, 13);
			this->staticText1->TabIndex = 0;
			this->staticText1->Text = L"Title";
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(14, 48);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(43, 13);
			this->label1->TabIndex = 2;
			this->label1->Text = L"X Label";
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(14, 75);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(43, 13);
			this->label2->TabIndex = 4;
			this->label2->Text = L"Y Label";
			// 
			// groupBox2
			// 
			this->groupBox2->Controls->Add(this->checkBox_autoSetRange);
			this->groupBox2->Controls->Add(this->label3);
			this->groupBox2->Controls->Add(this->label4);
			this->groupBox2->Controls->Add(this->textBox_yMax);
			this->groupBox2->Controls->Add(this->textBox_yMin);
			this->groupBox2->Location = System::Drawing::Point(216, 7);
			this->groupBox2->Name = L"groupBox2";
			this->groupBox2->Size = System::Drawing::Size(130, 106);
			this->groupBox2->TabIndex = 13;
			this->groupBox2->TabStop = false;
			this->groupBox2->Text = L"Range";
			// 
			// checkBox_autoSetRange
			// 
			this->checkBox_autoSetRange->AutoSize = true;
			this->checkBox_autoSetRange->Checked = true;
			this->checkBox_autoSetRange->CheckState = System::Windows::Forms::CheckState::Checked;
			this->checkBox_autoSetRange->Location = System::Drawing::Point(12, 17);
			this->checkBox_autoSetRange->Name = L"checkBox_autoSetRange";
			this->checkBox_autoSetRange->Size = System::Drawing::Size(95, 17);
			this->checkBox_autoSetRange->TabIndex = 6;
			this->checkBox_autoSetRange->Text = L"Auto-set range";
			this->checkBox_autoSetRange->UseVisualStyleBackColor = true;
			this->checkBox_autoSetRange->CheckedChanged += gcnew System::EventHandler(this, &DataGraphDialog::autoScaleCheckBox_CheckedChanged);
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Location = System::Drawing::Point(16, 47);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(34, 13);
			this->label3->TabIndex = 7;
			this->label3->Text = L"Y Min";
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Location = System::Drawing::Point(13, 74);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(37, 13);
			this->label4->TabIndex = 8;
			this->label4->Text = L"Y Max";
			// 
			// textBox_yMax
			// 
			this->textBox_yMax->Enabled = false;
			this->textBox_yMax->Location = System::Drawing::Point(56, 71);
			this->textBox_yMax->Name = L"textBox_yMax";
			this->textBox_yMax->Size = System::Drawing::Size(63, 20);
			this->textBox_yMax->TabIndex = 10;
			this->textBox_yMax->TextChanged += gcnew System::EventHandler(this, &DataGraphDialog::textBox_yMax_TextChanged);
			// 
			// textBox_yMin
			// 
			this->textBox_yMin->Enabled = false;
			this->textBox_yMin->Location = System::Drawing::Point(56, 44);
			this->textBox_yMin->Name = L"textBox_yMin";
			this->textBox_yMin->Size = System::Drawing::Size(63, 20);
			this->textBox_yMin->TabIndex = 9;
			this->textBox_yMin->TextChanged += gcnew System::EventHandler(this, &DataGraphDialog::textBox_yMin_TextChanged);
			// 
			// tabPage4
			// 
			this->tabPage4->Controls->Add(this->stopLoggingButton);
			this->tabPage4->Controls->Add(this->startLoggingButton);
			this->tabPage4->Controls->Add(this->logFileBrowseButton);
			this->tabPage4->Controls->Add(this->logFileTextBox);
			this->tabPage4->Location = System::Drawing::Point(4, 22);
			this->tabPage4->Name = L"tabPage4";
			this->tabPage4->Padding = System::Windows::Forms::Padding(3);
			this->tabPage4->Size = System::Drawing::Size(346, 178);
			this->tabPage4->TabIndex = 3;
			this->tabPage4->Text = L"Log File";
			this->tabPage4->UseVisualStyleBackColor = true;
			// 
			// stopLoggingButton
			// 
			this->stopLoggingButton->Enabled = false;
			this->stopLoggingButton->Location = System::Drawing::Point(120, 59);
			this->stopLoggingButton->Name = L"stopLoggingButton";
			this->stopLoggingButton->Size = System::Drawing::Size(88, 23);
			this->stopLoggingButton->TabIndex = 3;
			this->stopLoggingButton->Text = L"Stop Logging";
			this->stopLoggingButton->UseVisualStyleBackColor = true;
			this->stopLoggingButton->Click += gcnew System::EventHandler(this, &DataGraphDialog::stopLoggingButton_Click);
			// 
			// startLoggingButton
			// 
			this->startLoggingButton->Enabled = false;
			this->startLoggingButton->Location = System::Drawing::Point(17, 59);
			this->startLoggingButton->Name = L"startLoggingButton";
			this->startLoggingButton->Size = System::Drawing::Size(88, 23);
			this->startLoggingButton->TabIndex = 2;
			this->startLoggingButton->Text = L"Start Logging";
			this->startLoggingButton->UseVisualStyleBackColor = true;
			this->startLoggingButton->Click += gcnew System::EventHandler(this, &DataGraphDialog::startLoggingButton_Click);
			// 
			// logFileBrowseButton
			// 
			this->logFileBrowseButton->Location = System::Drawing::Point(256, 17);
			this->logFileBrowseButton->Name = L"logFileBrowseButton";
			this->logFileBrowseButton->Size = System::Drawing::Size(75, 23);
			this->logFileBrowseButton->TabIndex = 1;
			this->logFileBrowseButton->Text = L"Browse";
			this->logFileBrowseButton->UseVisualStyleBackColor = true;
			this->logFileBrowseButton->Click += gcnew System::EventHandler(this, &DataGraphDialog::logFileBrowseButton_Click);
			// 
			// logFileTextBox
			// 
			this->logFileTextBox->Location = System::Drawing::Point(8, 19);
			this->logFileTextBox->Name = L"logFileTextBox";
			this->logFileTextBox->ReadOnly = true;
			this->logFileTextBox->Size = System::Drawing::Size(242, 20);
			this->logFileTextBox->TabIndex = 0;
			this->logFileTextBox->Text = L"Click \"Browse\" to select a file";
			// 
			// timer1
			// 
			this->timer1->Tick += gcnew System::EventHandler(this, &DataGraphDialog::timer1_Tick);
			// 
			// saveFileDialog1
			// 
			this->saveFileDialog1->Filter = L"CSV Files (*.csv)|*.csv|Log Files (*.log)|*.log|Text Files (*.txt)|*.txt|All File" 
				L"s (*.*)|*.*";
			// 
			// DataGraphDialog
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(354, 205);
			this->Controls->Add(this->tabControl);
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->MaximizeBox = false;
			this->MinimizeBox = false;
			this->MinimumSize = System::Drawing::Size(370, 243);
			this->Name = L"DataGraphDialog";
			this->Text = L"Data Graph";
			this->Load += gcnew System::EventHandler(this, &DataGraphDialog::DataGraphDialog_Load);
			this->FormClosed += gcnew System::Windows::Forms::FormClosedEventHandler(this, &DataGraphDialog::DataGraphDialog_FormClosed);
			this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &DataGraphDialog::DataGraphDialog_FormClosing_1);
			this->Resize += gcnew System::EventHandler(this, &DataGraphDialog::DataGraphDialog_Resize);
			this->tabControl->ResumeLayout(false);
			this->tabPage1->ResumeLayout(false);
			this->tabPage2->ResumeLayout(false);
			this->tabPage3->ResumeLayout(false);
			this->tabPage3->PerformLayout();
			this->groupBox1->ResumeLayout(false);
			this->groupBox1->PerformLayout();
			this->groupBox2->ResumeLayout(false);
			this->groupBox2->PerformLayout();
			this->tabPage4->ResumeLayout(false);
			this->tabPage4->PerformLayout();
			this->ResumeLayout(false);

		}
#pragma endregion
	private: System::Void DataGraphDialog_Resize(System::Object^  sender, System::EventArgs^  e) {
				 this->tabControl->Size = this->ClientSize;
			 }
			 // When form is closed, unload event handlers, change back to the default size, etc.
	private: System::Void DataGraphDialog_FormClosing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e) {
			 }
	private: System::Void DataGraphDialog_FormClosed(System::Object^  sender, System::Windows::Forms::FormClosedEventArgs^  e) {
				 
			 }
	private: System::Void DataGraphDialog_Load(System::Object^  sender, System::EventArgs^  e) {
			 }
private: System::Void tabControl_Resize(System::Object^  sender, System::EventArgs^  e) {		 
			 this->graphControl->Size = this->tabControl->SelectedTab->ClientSize;
			 this->dataItemTreeView->Size = this->tabControl->SelectedTab->ClientSize;
		 }
private: System::Void tabPage3_Click(System::Object^  sender, System::EventArgs^  e) {
		 }
private: System::Void autoScaleCheckBox_CheckedChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 if( this->checkBox_autoSetRange->Checked )
			 {
				this->textBox_yMax->Enabled = false;
				this->textBox_yMin->Enabled = false;
			 }			
			 else
			 {
				this->textBox_yMax->Enabled = true;
				this->textBox_yMin->Enabled = true;
			 }

			 this->button_applyChanges->Enabled = true;

		 }
private: System::Void dataItemTreeView_AfterCheck(System::Object^  sender, System::Windows::Forms::TreeViewEventArgs^  e) 
		 {
			 // Clear and update the graph
			 this->ClearGraph();
			 this->InitializeGraph();
		 }
private: System::Void timer1_Tick(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->graphTime->Stop();
			 			 
			 time = this->graphTime->Elapsed;

			 if( loggingEnabled )
			 {
				logFile->Write(time.ToString());
				logFile->Write(L",");
			 }

			 // Update graph contents based on most recently received data
			 for( UInt32 i = 0; i < this->dataListCount; i++ )
			 {
				 FirmwareItem^ current_item = this->firmware->GetDataItem(this->dataItemIndexes[i]);
				 String^ data_type = current_item->GetDataType();
				 double data;
				 if( data_type == L"float" )
				 {
					 data = (double)current_item->GetFloatData()*(double)current_item->GetScaleFactor();
				 }
				 else if( data_type == L"int16" || data_type == L"int32" )
				 {
					 data = (double)current_item->GetIntData()*(double)current_item->GetScaleFactor();
				 }
				 else if( data_type == L"uint16" || data_type == L"uint32" )
				 {
					 data = (double)current_item->GetUIntData()*(double)current_item->GetScaleFactor();
				 }
				 else if( data_type == L"binary" || data_type == L"en/dis" )
				 {
					 bool value = current_item->GetBinaryData();
					 if( value )
						 data = 1.0;
					 else
						 data = 0.0;
				 }
				 else
				 {
					 data = -1.0;
				 }

				 this->dataGraphList[i]->Add(time, data);

				 // If logging is enabled, write the data to file
				 if( loggingEnabled )
				 {
					 logFile->Write(data.ToString());
					 if( i < this->dataListCount - 1 )
					 {
						 logFile->Write(L",");
					 }
				 }

			 }

			 if( loggingEnabled )
			 {
				logFile->Write(logFile->NewLine);				
			 }

			 this->RefreshGraph();
		 }
private: System::Void button_applyChanges_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->RetrieveGraphSettingsFromDialog();
			 this->ClearGraph();
			 this->InitializeGraph();

			 this->button_applyChanges->Enabled = false;
		 }
private: System::Void textBox_yMin_TextChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->button_applyChanges->Enabled = true;
		 }
private: System::Void textBox_yMax_TextChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->button_applyChanges->Enabled = true;
		 }
private: System::Void textBox_historySize_TextChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->button_applyChanges->Enabled = true;
		 }
private: System::Void textBox_title_TextChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->button_applyChanges->Enabled = true;
		 }
private: System::Void textBox_xlabel_TextChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->button_applyChanges->Enabled = true;
		 }
private: System::Void textBox_ylabel_TextChanged(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->button_applyChanges->Enabled = true;
		 }
private: System::Void logFileBrowseButton_Click(System::Object^  sender, System::EventArgs^  e) {
			 // Open save file dialog
			 this->saveFileDialog1->ShowDialog();
			 
			 if( this->saveFileDialog1->FileName != "" )
			 {
				 this->logFileTextBox->Text = this->saveFileDialog1->FileName;
				 this->startLoggingButton->Enabled = true;
			 }
		 }
private: System::Void startLoggingButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 if( this->logFile != nullptr )
			 {
				 logFile->Close();
			 }

			 try
			 {
				 this->logFile = gcnew StreamWriter(saveFileDialog1->FileName);
			 }
			 catch( Exception^ /*e*/ )
			 {
				 return;
			 }

			 logFile->Write(L"Time (s),");			 

			 // If file opened properly, write heading to file
			 for( UInt32 i = 0; i < this->dataListCount; i++ )
			 {
				 FirmwareItem^ current_item = this->firmware->GetDataItem(this->dataItemIndexes[i]);
				 logFile->Write(current_item->Text);
				 if( i < (dataListCount-1) )
				 {
					 logFile->Write(L",");
				 }
			 }

			 logFile->Write(logFile->NewLine);
			 
			 this->loggingEnabled = true;
			 this->startLoggingButton->Enabled = false;
			 this->stopLoggingButton->Enabled = true;
			 this->logFileBrowseButton->Enabled = false;
		 }
private: System::Void stopLoggingButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->logFile->Close();

			 this->loggingEnabled = false;
			 this->startLoggingButton->Enabled = true;
			 this->stopLoggingButton->Enabled = false;
			 this->logFileBrowseButton->Enabled = true;
		 }
private: System::Void DataGraphDialog_FormClosing_1(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e) {
			 if( this->logFile != nullptr )
			 {
				 logFile->Close();
				 loggingEnabled = false;
			 }
		 }
};
}
