#pragma once


namespace CHRInterface {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace ZedGraph;

	using namespace System::Xml;
	using namespace System::IO;

	using namespace DotNetMatrix;

#define	MAXIMUM_DATA_GRAPHS		10

#define	MAXIMUM_PACKET_RETRY	3

// Maximum number of datapoints that can be stored for use in magnetometer calibration
#define	MAXIMUM_MAG_DATA_POINTS		10000

	/// <summary>
	/// Summary for Form1
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class Form1 : public System::Windows::Forms::Form
	{
	public:
		Form1(void)
		{
			InitializeComponent();

			version = L"2.1.0";
			this->Text = "CHRobotics Serial Interface v" + version + " - Disconnected";

			configTextBox = gcnew TextBox();
			configComboBox = gcnew ComboBox();

			firmwareProgrammer = gcnew STM32Programmer();

			configTextBox->Visible = false;
			configComboBox->Visible = false;

			SelectedItemIndex = -1;
			SelectedFirmwareIndex = -1;

			packetsToSend = gcnew cli::array<SerialPacket^>(100);
			this->packetCount = 0;
			this->packetRetryCount = 0;

			// Items used for reading configuration data from the sensor
			this->readingConfigData = false;

			// Flag indicating whether raw magnetometer data is being collected for use in calibration
			this->magDataCollectionEnabled = false;
			
			// Array for storing raw magnetometer data for calibration
			rawMagData = gcnew cli::array<double,2>(MAXIMUM_MAG_DATA_POINTS,3);
			rawMagDataPointer = 0;

			// Arrays for storing computed magnetometer calibration data
			bias = gcnew cli::array<Int16^,1>(3);
			calMat = gcnew cli::array<float,2>(3,3);

			// Event handler to signal a change in a configuration combo box setting
			configComboBox->SelectedIndexChanged += gcnew EventHandler(this, &Form1::configCombo_SelectionChanged);

			this->treeConfig->Controls->Add(configTextBox);
			this->treeConfig->Controls->Add(configComboBox);

			// Add event handlers for firmware programmer
			this->firmwareProgrammer->OnProgressUpdate += gcnew FirmwareProgressEventHandler( this, &Form1::firmwareProgressUpdate_eventHandler );
			this->firmwareProgrammer->OnStatusChange += gcnew FirmwareStatusEventHandler( this, &Form1::firmwareStatusUpdate_eventHandler );

			// Create serial port object
			serialConnector = gcnew SerialConnector();

			// Add event handlers for dealing with packets
			serialConnector->OnSerialPacketError += gcnew SerialPacketErrorEventHandler( this, &Form1::SerialPacketError_eventHandler );
			serialConnector->OnSerialPacketReceived += gcnew SerialPacketReceivedEventHandler( this, &Form1::SerialPacketReceived_eventHandler );
	
			// Populate combo boxes for serial communication settings
			cli::array<String^>^ portNames = serialConnector->GetPortNames();
			for each( String^ portName in portNames )
			{
				this->serialComboBox->Items->Add( portName );
			}
			this->serialComboBox->SelectedIndex = 0;

			this->serialBaudComboBox->Items->Add(L"9600");
			this->serialBaudComboBox->Items->Add(L"14400");
			this->serialBaudComboBox->Items->Add(L"19200");
			this->serialBaudComboBox->Items->Add(L"38400");
			this->serialBaudComboBox->Items->Add(L"57600");
			this->serialBaudComboBox->Items->Add(L"115200");
			
			this->serialBaudComboBox->SelectedIndex = 5;

			this->serialParityComboBox->Items->Add(L"None");
			this->serialParityComboBox->Items->Add(L"Even");
			this->serialParityComboBox->Items->Add(L"Odd");

			this->serialParityComboBox->SelectedIndex = 0;

			this->serialStopBitsComboBox->Items->Add(L"1");
			this->serialStopBitsComboBox->Items->Add(L"2");
			this->serialStopBitsComboBox->SelectedIndex = 0;

			// Add event handlers for tree views
			treeConfig->NodeMouseClick += gcnew TreeNodeMouseClickEventHandler(this, &Form1::firmwareBox_NodeClicked);
			treeConfig->LostFocus += gcnew EventHandler(this, &Form1::firmwareBox_LostFocus);
			treeConfig->Invalidated += gcnew InvalidateEventHandler(this, &Form1::treeBox_Invalidated);

			treeConfig->MouseWheel += gcnew MouseEventHandler(this, &Form1::firmwareBox_MouseWheel);

			treeConfig->AfterCollapse += gcnew TreeViewEventHandler(this, &Form1::firmwareBox_Altered);
			treeConfig->AfterExpand += gcnew TreeViewEventHandler(this, &Form1::firmwareBox_Altered);

			// Create data graph array
			dataGraphs = gcnew cli::array<DataGraphDialog^>(MAXIMUM_DATA_GRAPHS);
			currentGraphCount = 0;

			// Open current directory and look for firmware.  Load all relevant firmware XML files for later use.
			current_directory = System::Environment::CurrentDirectory;
			firmware_directory = current_directory + L"\\firmware";

			FirmwareArray = gcnew cli::array<FirmwareDefinition^>(100);
			FirmwareCount = 0;

			try {
				cli::array<String^>^ files = Directory::GetFiles(firmware_directory, L"*.XML");
				int numFiles = files->Length;

				// If .XML files were found, attempt to read them in to define program operation
				if( numFiles == 0 )
				{
					statusBox->Text = L"No firmware definitions were found.";
				}
				else
				{
					FirmwareArray[FirmwareCount] = gcnew FirmwareDefinition();

					for( int i = 0; i < numFiles; i++ )
					{
						// Try parsing this XML file to extract firmware data
						try
						{
							if( FirmwareArray[FirmwareCount]->XML_parse(files[i]) )
							{
								FirmwareCount++;
								FirmwareArray[FirmwareCount] = gcnew FirmwareDefinition();
							}
						}
						catch(Exception^ e)
						{
							statusBox->Text += (L"Firmware parsing error:\r\n" + e->ToString());
						}
					}

					if( FirmwareCount == 0 )
					{
						statusBox->Text += L"\r\n\r\nFound firmware definitions, but there were parsing errors.\r\n";
					}
				}

				

			}
			catch( DirectoryNotFoundException^ /*e*/ ) {
				statusBox->Text = L"Error: Firmware directory does not exist at:\r\n" + firmware_directory;
			}
			catch( System::Exception^ e ) {
//				statusBox->Text = L"Error: Unable to obtain firmware definitions in directory:\r\n" + firmware_directory;
				statusBox->Text = e->ToString();
			}
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~Form1()
		{
			if (components)
			{
				delete components;
			}
		}
#pragma region Private Member Variables

	private: delegate void UpdateFormDelegate();
	private: delegate void UpdateFormTextDelegate( String^ text );
	private: delegate void UpdateFormUIntDelegate( UInt32 integer );
	private: delegate void UpdateFormIntDelegate( int integer );
	private: delegate void UpdateStatusDelegate( String^ text, System::Drawing::Color color );
	private: delegate void UpdateFormUCharDelegate( unsigned char character );
	private: delegate void UpdateFormSerialPacketDelegate( SerialPacket^ packet );
	private: delegate void UpdateFormBoolDelegate( bool value );

	private: String^ current_directory;
	private: String^ version;
	private: String^ firmware_directory;
	private: cli::array<FirmwareDefinition^>^ FirmwareArray;
	private: int FirmwareCount;
	private: int SelectedFirmwareIndex;
	private: TextBox^ configTextBox;
	private: ComboBox^ configComboBox;
	private: int SelectedItemIndex;

	// When reading configuration registers from device, this value indicates how many register have been
	// read (allows progress bar to be updated).
	private: int registersRead;

	private: cli::array<SerialPacket^>^ packetsToSend;
	private: int packetRetryCount;		// Number of times the current packet has been sent (retried)
	private: int packetCount;

	private: bool readingConfigData;
	private: bool magDataCollectionEnabled;		// Flag that specifies whether magnetometer data is currently being collected for calibration.
	private: cli::array<double,2>^ rawMagData;
	private: UInt32 rawMagDataPointer;				// Pointer to indicate how much raw magnetometer data has been collected

	private: cli::array<Int16^,1>^ bias;
	private: cli::array<float,2>^ calMat;

	private: cli::array<DataGraphDialog^>^ dataGraphs;
	private: UInt32 currentGraphCount;
	
	// Dialog for displaying progress while reading configuration data from a sensor
	private: ProgressView^ configProgress;
			 private: STM32Programmer^ firmwareProgrammer;

	
	private: System::Windows::Forms::TabControl^  tabControl;
	protected: 
	private: System::Windows::Forms::TabPage^  tabSerialSetting;
	private: System::Windows::Forms::TabPage^  tabCommands;
	private: System::Windows::Forms::TabPage^  tabData;
	private: System::Windows::Forms::TabPage^  tabConfig;
	private: System::Windows::Forms::GroupBox^  groupBox1;
private: System::Windows::Forms::Button^  flashWriteButton;


	private: System::Windows::Forms::Button^  readButton;
private: System::Windows::Forms::Button^  writeButton;



	private: System::Windows::Forms::TreeView^  treeConfig;



	private: System::Windows::Forms::TreeView^  treeData;
	private: System::Windows::Forms::RichTextBox^  statusBox;

	private: System::Windows::Forms::Button^  serialDisconnectButton;
	private: System::Windows::Forms::Button^  serialConnectButton;
	private: System::Windows::Forms::ComboBox^  serialBaudComboBox;
	private: System::Windows::Forms::ComboBox^  serialComboBox;

	private: System::Windows::Forms::Button^  getFirmwareVersionButton;
	private: System::Windows::Forms::GroupBox^  groupBox3;

	private: System::Windows::Forms::Label^  label2;
	private: System::Windows::Forms::Label^  label1;


	private: System::Windows::Forms::Label^  label5;
	private: System::Windows::Forms::ComboBox^  serialStopBitsComboBox;

	private: System::Windows::Forms::Label^  label4;
	private: System::Windows::Forms::ComboBox^  serialParityComboBox;

	private: System::ComponentModel::IContainer^  components;
private: System::Windows::Forms::TextBox^  firmwareVersionBox;
private: System::Windows::Forms::GroupBox^  groupBox2;
private: System::Windows::Forms::GroupBox^  groupBox4;
private: System::Windows::Forms::Button^  dataGraphButton;
private: System::Windows::Forms::Timer^  packetResponseTimer;
private: System::Windows::Forms::Label^  label3;
private: System::Windows::Forms::ListBox^  commandsListBox;

private: System::Windows::Forms::Label^  label6;
private: System::Windows::Forms::Label^  label8;
private: System::Windows::Forms::Label^  label7;
private: System::Windows::Forms::Label^  label10;
private: System::Windows::Forms::Label^  label9;
private: System::Windows::Forms::GroupBox^  groupBox5;
private: System::Windows::Forms::TextBox^  firmwareVersionBox2;
private: System::Windows::Forms::TabPage^  tabCalibration;
private: System::Windows::Forms::Label^  labelMagDataPoints;

private: System::Windows::Forms::Label^  label12;
private: System::Windows::Forms::Button^  buttonComputeCalibration;
private: System::Windows::Forms::Label^  labelCalibrationStatus;
private: System::Windows::Forms::Label^  label13;
private: System::Windows::Forms::Button^  buttonResetDataCollection;
private: System::Windows::Forms::Button^  buttonStopDataCollection;
private: System::Windows::Forms::Button^  buttonStartDataCollection;
private: System::Windows::Forms::GroupBox^  groupBox6;
private: System::Windows::Forms::Label^  label14;
private: System::Windows::Forms::Label^  label15;
private: System::Windows::Forms::Label^  label16;
private: System::Windows::Forms::Label^  label17;
private: System::Windows::Forms::Label^  label18;
private: System::Windows::Forms::Button^  buttonWriteMagConfigToRAM;

private: System::Windows::Forms::TextBox^  magBiasZ;
private: System::Windows::Forms::TextBox^  magMatrix12;
private: System::Windows::Forms::TextBox^  magBiasY;
private: System::Windows::Forms::TextBox^  magMatrix00;
private: System::Windows::Forms::TextBox^  magBiasX;
private: System::Windows::Forms::TextBox^  magMatrix01;
private: System::Windows::Forms::TextBox^  magMatrix22;
private: System::Windows::Forms::TextBox^  magMatrix02;
private: System::Windows::Forms::TextBox^  magMatrix21;
private: System::Windows::Forms::TextBox^  magMatrix10;
private: System::Windows::Forms::TextBox^  magMatrix20;
private: System::Windows::Forms::TextBox^  magMatrix11;



private: System::Windows::Forms::OpenFileDialog^  openFileDialog1;
private: System::Windows::Forms::TextBox^  firmwareFileTextBox;
private: System::Windows::Forms::Button^  writeFirmwareButton;
private: System::Windows::Forms::Button^  firmwareFileBrowseButton;
private: System::Windows::Forms::GroupBox^  groupBox7;
private: System::Windows::Forms::Label^  label11;
private: System::Windows::Forms::Label^  firmwareSizeLabel;

private: System::Windows::Forms::Label^  firmwareStatusLabel;
private: System::Windows::Forms::ProgressBar^  firmwareProgressBar;










	private: SerialConnector^ serialConnector;

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>

#pragma endregion

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^  resources = (gcnew System::ComponentModel::ComponentResourceManager(Form1::typeid));
			this->tabControl = (gcnew System::Windows::Forms::TabControl());
			this->tabSerialSetting = (gcnew System::Windows::Forms::TabPage());
			this->label10 = (gcnew System::Windows::Forms::Label());
			this->groupBox3 = (gcnew System::Windows::Forms::GroupBox());
			this->label5 = (gcnew System::Windows::Forms::Label());
			this->serialComboBox = (gcnew System::Windows::Forms::ComboBox());
			this->serialBaudComboBox = (gcnew System::Windows::Forms::ComboBox());
			this->serialStopBitsComboBox = (gcnew System::Windows::Forms::ComboBox());
			this->label2 = (gcnew System::Windows::Forms::Label());
			this->label1 = (gcnew System::Windows::Forms::Label());
			this->label4 = (gcnew System::Windows::Forms::Label());
			this->serialParityComboBox = (gcnew System::Windows::Forms::ComboBox());
			this->groupBox2 = (gcnew System::Windows::Forms::GroupBox());
			this->serialDisconnectButton = (gcnew System::Windows::Forms::Button());
			this->serialConnectButton = (gcnew System::Windows::Forms::Button());
			this->groupBox4 = (gcnew System::Windows::Forms::GroupBox());
			this->firmwareVersionBox = (gcnew System::Windows::Forms::TextBox());
			this->getFirmwareVersionButton = (gcnew System::Windows::Forms::Button());
			this->groupBox7 = (gcnew System::Windows::Forms::GroupBox());
			this->firmwareProgressBar = (gcnew System::Windows::Forms::ProgressBar());
			this->firmwareStatusLabel = (gcnew System::Windows::Forms::Label());
			this->firmwareSizeLabel = (gcnew System::Windows::Forms::Label());
			this->writeFirmwareButton = (gcnew System::Windows::Forms::Button());
			this->label11 = (gcnew System::Windows::Forms::Label());
			this->firmwareFileBrowseButton = (gcnew System::Windows::Forms::Button());
			this->firmwareFileTextBox = (gcnew System::Windows::Forms::TextBox());
			this->tabCommands = (gcnew System::Windows::Forms::TabPage());
			this->label6 = (gcnew System::Windows::Forms::Label());
			this->label3 = (gcnew System::Windows::Forms::Label());
			this->commandsListBox = (gcnew System::Windows::Forms::ListBox());
			this->tabData = (gcnew System::Windows::Forms::TabPage());
			this->label8 = (gcnew System::Windows::Forms::Label());
			this->label7 = (gcnew System::Windows::Forms::Label());
			this->dataGraphButton = (gcnew System::Windows::Forms::Button());
			this->treeData = (gcnew System::Windows::Forms::TreeView());
			this->tabConfig = (gcnew System::Windows::Forms::TabPage());
			this->label9 = (gcnew System::Windows::Forms::Label());
			this->groupBox5 = (gcnew System::Windows::Forms::GroupBox());
			this->firmwareVersionBox2 = (gcnew System::Windows::Forms::TextBox());
			this->treeConfig = (gcnew System::Windows::Forms::TreeView());
			this->groupBox1 = (gcnew System::Windows::Forms::GroupBox());
			this->flashWriteButton = (gcnew System::Windows::Forms::Button());
			this->readButton = (gcnew System::Windows::Forms::Button());
			this->writeButton = (gcnew System::Windows::Forms::Button());
			this->tabCalibration = (gcnew System::Windows::Forms::TabPage());
			this->labelMagDataPoints = (gcnew System::Windows::Forms::Label());
			this->label12 = (gcnew System::Windows::Forms::Label());
			this->buttonComputeCalibration = (gcnew System::Windows::Forms::Button());
			this->labelCalibrationStatus = (gcnew System::Windows::Forms::Label());
			this->label13 = (gcnew System::Windows::Forms::Label());
			this->buttonResetDataCollection = (gcnew System::Windows::Forms::Button());
			this->buttonStopDataCollection = (gcnew System::Windows::Forms::Button());
			this->buttonStartDataCollection = (gcnew System::Windows::Forms::Button());
			this->groupBox6 = (gcnew System::Windows::Forms::GroupBox());
			this->label14 = (gcnew System::Windows::Forms::Label());
			this->label15 = (gcnew System::Windows::Forms::Label());
			this->label16 = (gcnew System::Windows::Forms::Label());
			this->label17 = (gcnew System::Windows::Forms::Label());
			this->label18 = (gcnew System::Windows::Forms::Label());
			this->buttonWriteMagConfigToRAM = (gcnew System::Windows::Forms::Button());
			this->magBiasZ = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix12 = (gcnew System::Windows::Forms::TextBox());
			this->magBiasY = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix00 = (gcnew System::Windows::Forms::TextBox());
			this->magBiasX = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix01 = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix22 = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix02 = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix21 = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix10 = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix20 = (gcnew System::Windows::Forms::TextBox());
			this->magMatrix11 = (gcnew System::Windows::Forms::TextBox());
			this->statusBox = (gcnew System::Windows::Forms::RichTextBox());
			this->packetResponseTimer = (gcnew System::Windows::Forms::Timer(this->components));
			this->openFileDialog1 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->tabControl->SuspendLayout();
			this->tabSerialSetting->SuspendLayout();
			this->groupBox3->SuspendLayout();
			this->groupBox2->SuspendLayout();
			this->groupBox4->SuspendLayout();
			this->groupBox7->SuspendLayout();
			this->tabCommands->SuspendLayout();
			this->tabData->SuspendLayout();
			this->tabConfig->SuspendLayout();
			this->groupBox5->SuspendLayout();
			this->groupBox1->SuspendLayout();
			this->tabCalibration->SuspendLayout();
			this->groupBox6->SuspendLayout();
			this->SuspendLayout();
			// 
			// tabControl
			// 
			this->tabControl->Controls->Add(this->tabSerialSetting);
			this->tabControl->Controls->Add(this->tabCommands);
			this->tabControl->Controls->Add(this->tabData);
			this->tabControl->Controls->Add(this->tabConfig);
			this->tabControl->Controls->Add(this->tabCalibration);
			this->tabControl->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 8.25F, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->tabControl->Location = System::Drawing::Point(0, 0);
			this->tabControl->Name = L"tabControl";
			this->tabControl->SelectedIndex = 0;
			this->tabControl->Size = System::Drawing::Size(414, 391);
			this->tabControl->TabIndex = 0;
			// 
			// tabSerialSetting
			// 
			this->tabSerialSetting->Controls->Add(this->label10);
			this->tabSerialSetting->Controls->Add(this->groupBox3);
			this->tabSerialSetting->Controls->Add(this->groupBox2);
			this->tabSerialSetting->Controls->Add(this->groupBox4);
			this->tabSerialSetting->Controls->Add(this->groupBox7);
			this->tabSerialSetting->Location = System::Drawing::Point(4, 22);
			this->tabSerialSetting->Name = L"tabSerialSetting";
			this->tabSerialSetting->Padding = System::Windows::Forms::Padding(3);
			this->tabSerialSetting->Size = System::Drawing::Size(406, 365);
			this->tabSerialSetting->TabIndex = 0;
			this->tabSerialSetting->Text = L"Serial Settings";
			this->tabSerialSetting->UseVisualStyleBackColor = true;
			this->tabSerialSetting->Click += gcnew System::EventHandler(this, &Form1::tabSerialSetting_Click);
			// 
			// label10
			// 
			this->label10->AutoSize = true;
			this->label10->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->label10->ForeColor = System::Drawing::Color::Navy;
			this->label10->Location = System::Drawing::Point(8, 3);
			this->label10->Name = L"label10";
			this->label10->Size = System::Drawing::Size(193, 24);
			this->label10->TabIndex = 12;
			this->label10->Text = L"Serial Port Configuration";
			this->label10->TextAlign = System::Drawing::ContentAlignment::MiddleCenter;
			this->label10->UseCompatibleTextRendering = true;
			// 
			// groupBox3
			// 
			this->groupBox3->Controls->Add(this->label5);
			this->groupBox3->Controls->Add(this->serialComboBox);
			this->groupBox3->Controls->Add(this->serialBaudComboBox);
			this->groupBox3->Controls->Add(this->serialStopBitsComboBox);
			this->groupBox3->Controls->Add(this->label2);
			this->groupBox3->Controls->Add(this->label1);
			this->groupBox3->Controls->Add(this->label4);
			this->groupBox3->Controls->Add(this->serialParityComboBox);
			this->groupBox3->Location = System::Drawing::Point(6, 30);
			this->groupBox3->Name = L"groupBox3";
			this->groupBox3->Size = System::Drawing::Size(207, 137);
			this->groupBox3->TabIndex = 7;
			this->groupBox3->TabStop = false;
			this->groupBox3->Text = L"Serial Port Settings";
			// 
			// label5
			// 
			this->label5->AutoSize = true;
			this->label5->Location = System::Drawing::Point(7, 103);
			this->label5->Name = L"label5";
			this->label5->Size = System::Drawing::Size(49, 13);
			this->label5->TabIndex = 10;
			this->label5->Text = L"Stop Bits";
			// 
			// serialComboBox
			// 
			this->serialComboBox->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->serialComboBox->FormattingEnabled = true;
			this->serialComboBox->Location = System::Drawing::Point(62, 19);
			this->serialComboBox->Name = L"serialComboBox";
			this->serialComboBox->Size = System::Drawing::Size(108, 21);
			this->serialComboBox->TabIndex = 0;
			// 
			// serialBaudComboBox
			// 
			this->serialBaudComboBox->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->serialBaudComboBox->FormattingEnabled = true;
			this->serialBaudComboBox->IntegralHeight = false;
			this->serialBaudComboBox->Location = System::Drawing::Point(62, 46);
			this->serialBaudComboBox->Name = L"serialBaudComboBox";
			this->serialBaudComboBox->Size = System::Drawing::Size(108, 21);
			this->serialBaudComboBox->TabIndex = 3;
			// 
			// serialStopBitsComboBox
			// 
			this->serialStopBitsComboBox->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->serialStopBitsComboBox->FormattingEnabled = true;
			this->serialStopBitsComboBox->Location = System::Drawing::Point(62, 100);
			this->serialStopBitsComboBox->Name = L"serialStopBitsComboBox";
			this->serialStopBitsComboBox->Size = System::Drawing::Size(47, 21);
			this->serialStopBitsComboBox->TabIndex = 9;
			// 
			// label2
			// 
			this->label2->AutoSize = true;
			this->label2->Location = System::Drawing::Point(24, 49);
			this->label2->Name = L"label2";
			this->label2->Size = System::Drawing::Size(32, 13);
			this->label2->TabIndex = 6;
			this->label2->Text = L"Baud";
			// 
			// label1
			// 
			this->label1->AutoSize = true;
			this->label1->Location = System::Drawing::Point(30, 22);
			this->label1->Name = L"label1";
			this->label1->Size = System::Drawing::Size(26, 13);
			this->label1->TabIndex = 5;
			this->label1->Text = L"Port";
			this->label1->Click += gcnew System::EventHandler(this, &Form1::label1_Click);
			// 
			// label4
			// 
			this->label4->AutoSize = true;
			this->label4->Location = System::Drawing::Point(23, 76);
			this->label4->Name = L"label4";
			this->label4->Size = System::Drawing::Size(33, 13);
			this->label4->TabIndex = 8;
			this->label4->Text = L"Parity";
			// 
			// serialParityComboBox
			// 
			this->serialParityComboBox->DropDownStyle = System::Windows::Forms::ComboBoxStyle::DropDownList;
			this->serialParityComboBox->FormattingEnabled = true;
			this->serialParityComboBox->Location = System::Drawing::Point(62, 73);
			this->serialParityComboBox->Name = L"serialParityComboBox";
			this->serialParityComboBox->Size = System::Drawing::Size(69, 21);
			this->serialParityComboBox->TabIndex = 7;
			// 
			// groupBox2
			// 
			this->groupBox2->Controls->Add(this->serialDisconnectButton);
			this->groupBox2->Controls->Add(this->serialConnectButton);
			this->groupBox2->Location = System::Drawing::Point(220, 30);
			this->groupBox2->Name = L"groupBox2";
			this->groupBox2->Size = System::Drawing::Size(176, 62);
			this->groupBox2->TabIndex = 10;
			this->groupBox2->TabStop = false;
			this->groupBox2->Text = L"Serial Controls";
			// 
			// serialDisconnectButton
			// 
			this->serialDisconnectButton->Cursor = System::Windows::Forms::Cursors::Default;
			this->serialDisconnectButton->Enabled = false;
			this->serialDisconnectButton->FlatAppearance->BorderColor = System::Drawing::Color::White;
			this->serialDisconnectButton->FlatAppearance->BorderSize = 0;
			this->serialDisconnectButton->FlatAppearance->MouseDownBackColor = System::Drawing::Color::Transparent;
			this->serialDisconnectButton->FlatAppearance->MouseOverBackColor = System::Drawing::Color::Transparent;
			this->serialDisconnectButton->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->serialDisconnectButton->Location = System::Drawing::Point(89, 22);
			this->serialDisconnectButton->Name = L"serialDisconnectButton";
			this->serialDisconnectButton->Size = System::Drawing::Size(68, 23);
			this->serialDisconnectButton->TabIndex = 1;
			this->serialDisconnectButton->Text = L"Disconnect";
			this->serialDisconnectButton->UseVisualStyleBackColor = true;
			this->serialDisconnectButton->Click += gcnew System::EventHandler(this, &Form1::serialDisconnectButton_Click);
			// 
			// serialConnectButton
			// 
			this->serialConnectButton->Cursor = System::Windows::Forms::Cursors::Default;
			this->serialConnectButton->FlatAppearance->BorderColor = System::Drawing::Color::White;
			this->serialConnectButton->FlatAppearance->BorderSize = 0;
			this->serialConnectButton->FlatAppearance->MouseDownBackColor = System::Drawing::Color::Transparent;
			this->serialConnectButton->FlatAppearance->MouseOverBackColor = System::Drawing::Color::Transparent;
			this->serialConnectButton->FlatStyle = System::Windows::Forms::FlatStyle::System;
			this->serialConnectButton->Location = System::Drawing::Point(6, 22);
			this->serialConnectButton->Name = L"serialConnectButton";
			this->serialConnectButton->Size = System::Drawing::Size(68, 23);
			this->serialConnectButton->TabIndex = 2;
			this->serialConnectButton->Text = L"Connect";
			this->serialConnectButton->UseVisualStyleBackColor = true;
			this->serialConnectButton->Click += gcnew System::EventHandler(this, &Form1::serialConnectButton_Click);
			// 
			// groupBox4
			// 
			this->groupBox4->Controls->Add(this->firmwareVersionBox);
			this->groupBox4->Controls->Add(this->getFirmwareVersionButton);
			this->groupBox4->Location = System::Drawing::Point(220, 99);
			this->groupBox4->Name = L"groupBox4";
			this->groupBox4->Size = System::Drawing::Size(175, 68);
			this->groupBox4->TabIndex = 11;
			this->groupBox4->TabStop = false;
			this->groupBox4->Text = L"Sensor Firmware Version";
			// 
			// firmwareVersionBox
			// 
			this->firmwareVersionBox->Location = System::Drawing::Point(22, 27);
			this->firmwareVersionBox->Name = L"firmwareVersionBox";
			this->firmwareVersionBox->ReadOnly = true;
			this->firmwareVersionBox->Size = System::Drawing::Size(39, 20);
			this->firmwareVersionBox->TabIndex = 9;
			// 
			// getFirmwareVersionButton
			// 
			this->getFirmwareVersionButton->Enabled = false;
			this->getFirmwareVersionButton->Location = System::Drawing::Point(67, 25);
			this->getFirmwareVersionButton->Name = L"getFirmwareVersionButton";
			this->getFirmwareVersionButton->Size = System::Drawing::Size(75, 23);
			this->getFirmwareVersionButton->TabIndex = 5;
			this->getFirmwareVersionButton->Text = L"Get Version";
			this->getFirmwareVersionButton->UseVisualStyleBackColor = true;
			this->getFirmwareVersionButton->Click += gcnew System::EventHandler(this, &Form1::getFirmwareVersionButton_Click);
			// 
			// groupBox7
			// 
			this->groupBox7->Controls->Add(this->firmwareProgressBar);
			this->groupBox7->Controls->Add(this->firmwareStatusLabel);
			this->groupBox7->Controls->Add(this->firmwareSizeLabel);
			this->groupBox7->Controls->Add(this->writeFirmwareButton);
			this->groupBox7->Controls->Add(this->label11);
			this->groupBox7->Controls->Add(this->firmwareFileBrowseButton);
			this->groupBox7->Controls->Add(this->firmwareFileTextBox);
			this->groupBox7->Location = System::Drawing::Point(6, 186);
			this->groupBox7->Name = L"groupBox7";
			this->groupBox7->Size = System::Drawing::Size(393, 162);
			this->groupBox7->TabIndex = 20;
			this->groupBox7->TabStop = false;
			this->groupBox7->Text = L"Reprogram Firmware";
			// 
			// firmwareProgressBar
			// 
			this->firmwareProgressBar->Location = System::Drawing::Point(12, 101);
			this->firmwareProgressBar->Name = L"firmwareProgressBar";
			this->firmwareProgressBar->Size = System::Drawing::Size(371, 23);
			this->firmwareProgressBar->Step = 1;
			this->firmwareProgressBar->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->firmwareProgressBar->TabIndex = 23;
			// 
			// firmwareStatusLabel
			// 
			this->firmwareStatusLabel->AutoSize = true;
			this->firmwareStatusLabel->Location = System::Drawing::Point(9, 134);
			this->firmwareStatusLabel->Name = L"firmwareStatusLabel";
			this->firmwareStatusLabel->Size = System::Drawing::Size(124, 13);
			this->firmwareStatusLabel->TabIndex = 22;
			this->firmwareStatusLabel->Text = L"Programming Status: Idle";
			// 
			// firmwareSizeLabel
			// 
			this->firmwareSizeLabel->AutoSize = true;
			this->firmwareSizeLabel->Location = System::Drawing::Point(10, 75);
			this->firmwareSizeLabel->Name = L"firmwareSizeLabel";
			this->firmwareSizeLabel->Size = System::Drawing::Size(101, 13);
			this->firmwareSizeLabel->TabIndex = 20;
			this->firmwareSizeLabel->Text = L"Firmware Size: 0 KB";
			// 
			// writeFirmwareButton
			// 
			this->writeFirmwareButton->Enabled = false;
			this->writeFirmwareButton->Location = System::Drawing::Point(293, 70);
			this->writeFirmwareButton->Name = L"writeFirmwareButton";
			this->writeFirmwareButton->Size = System::Drawing::Size(90, 23);
			this->writeFirmwareButton->TabIndex = 19;
			this->writeFirmwareButton->Text = L"Program Firmware";
			this->writeFirmwareButton->UseVisualStyleBackColor = true;
			this->writeFirmwareButton->Click += gcnew System::EventHandler(this, &Form1::writeFirmwareButton_Click);
			// 
			// label11
			// 
			this->label11->AutoSize = true;
			this->label11->Location = System::Drawing::Point(9, 22);
			this->label11->Name = L"label11";
			this->label11->Size = System::Drawing::Size(48, 13);
			this->label11->TabIndex = 11;
			this->label11->Text = L"HEX File";
			// 
			// firmwareFileBrowseButton
			// 
			this->firmwareFileBrowseButton->Location = System::Drawing::Point(308, 39);
			this->firmwareFileBrowseButton->Name = L"firmwareFileBrowseButton";
			this->firmwareFileBrowseButton->Size = System::Drawing::Size(75, 23);
			this->firmwareFileBrowseButton->TabIndex = 18;
			this->firmwareFileBrowseButton->Text = L"Browse";
			this->firmwareFileBrowseButton->UseVisualStyleBackColor = true;
			this->firmwareFileBrowseButton->Click += gcnew System::EventHandler(this, &Form1::firmwareFileBrowseButton_Click);
			// 
			// firmwareFileTextBox
			// 
			this->firmwareFileTextBox->Location = System::Drawing::Point(12, 41);
			this->firmwareFileTextBox->Name = L"firmwareFileTextBox";
			this->firmwareFileTextBox->ReadOnly = true;
			this->firmwareFileTextBox->Size = System::Drawing::Size(290, 20);
			this->firmwareFileTextBox->TabIndex = 17;
			this->firmwareFileTextBox->Text = L"Click \"Browse\" to select a file";
			// 
			// tabCommands
			// 
			this->tabCommands->Controls->Add(this->label6);
			this->tabCommands->Controls->Add(this->label3);
			this->tabCommands->Controls->Add(this->commandsListBox);
			this->tabCommands->Location = System::Drawing::Point(4, 22);
			this->tabCommands->Name = L"tabCommands";
			this->tabCommands->Padding = System::Windows::Forms::Padding(3, 2, 3, 2);
			this->tabCommands->Size = System::Drawing::Size(406, 365);
			this->tabCommands->TabIndex = 1;
			this->tabCommands->Text = L"Commands";
			this->tabCommands->UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this->label6->AutoSize = true;
			this->label6->Location = System::Drawing::Point(8, 31);
			this->label6->Name = L"label6";
			this->label6->Size = System::Drawing::Size(294, 13);
			this->label6->TabIndex = 2;
			this->label6->Text = L"Double-click an item below to send a command to the sensor";
			// 
			// label3
			// 
			this->label3->AutoSize = true;
			this->label3->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->label3->ForeColor = System::Drawing::Color::Navy;
			this->label3->Location = System::Drawing::Point(7, 3);
			this->label3->Name = L"label3";
			this->label3->Size = System::Drawing::Size(93, 24);
			this->label3->TabIndex = 1;
			this->label3->Text = L"Commands";
			this->label3->TextAlign = System::Drawing::ContentAlignment::MiddleCenter;
			this->label3->UseCompatibleTextRendering = true;
			// 
			// commandsListBox
			// 
			this->commandsListBox->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 10, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->commandsListBox->ForeColor = System::Drawing::Color::Navy;
			this->commandsListBox->FormattingEnabled = true;
			this->commandsListBox->ItemHeight = 16;
			this->commandsListBox->Location = System::Drawing::Point(8, 56);
			this->commandsListBox->Name = L"commandsListBox";
			this->commandsListBox->Size = System::Drawing::Size(389, 292);
			this->commandsListBox->Sorted = true;
			this->commandsListBox->TabIndex = 0;
			this->commandsListBox->MouseDoubleClick += gcnew System::Windows::Forms::MouseEventHandler(this, &Form1::commandsListBox_MouseDoubleClick);
			// 
			// tabData
			// 
			this->tabData->Controls->Add(this->label8);
			this->tabData->Controls->Add(this->label7);
			this->tabData->Controls->Add(this->dataGraphButton);
			this->tabData->Controls->Add(this->treeData);
			this->tabData->Location = System::Drawing::Point(4, 22);
			this->tabData->Name = L"tabData";
			this->tabData->Padding = System::Windows::Forms::Padding(3);
			this->tabData->Size = System::Drawing::Size(406, 365);
			this->tabData->TabIndex = 2;
			this->tabData->Text = L"Data";
			this->tabData->UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this->label8->AutoSize = true;
			this->label8->Location = System::Drawing::Point(8, 27);
			this->label8->Name = L"label8";
			this->label8->Size = System::Drawing::Size(360, 13);
			this->label8->TabIndex = 8;
			this->label8->Text = L"Select which data you would like to graph and click \"Create Graph\" below.";
			// 
			// label7
			// 
			this->label7->AutoSize = true;
			this->label7->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->label7->ForeColor = System::Drawing::Color::Navy;
			this->label7->Location = System::Drawing::Point(8, 3);
			this->label7->Name = L"label7";
			this->label7->Size = System::Drawing::Size(101, 24);
			this->label7->TabIndex = 7;
			this->label7->Text = L"Sensor Data";
			this->label7->TextAlign = System::Drawing::ContentAlignment::MiddleCenter;
			this->label7->UseCompatibleTextRendering = true;
			// 
			// dataGraphButton
			// 
			this->dataGraphButton->Enabled = false;
			this->dataGraphButton->Location = System::Drawing::Point(8, 330);
			this->dataGraphButton->Name = L"dataGraphButton";
			this->dataGraphButton->Size = System::Drawing::Size(191, 23);
			this->dataGraphButton->TabIndex = 6;
			this->dataGraphButton->Text = L"Create Graph from Selected Items";
			this->dataGraphButton->UseVisualStyleBackColor = true;
			this->dataGraphButton->Click += gcnew System::EventHandler(this, &Form1::dataGraphButton_Click);
			// 
			// treeData
			// 
			this->treeData->CheckBoxes = true;
			this->treeData->Location = System::Drawing::Point(8, 52);
			this->treeData->Name = L"treeData";
			this->treeData->Size = System::Drawing::Size(389, 272);
			this->treeData->TabIndex = 5;
			// 
			// tabConfig
			// 
			this->tabConfig->Controls->Add(this->label9);
			this->tabConfig->Controls->Add(this->groupBox5);
			this->tabConfig->Controls->Add(this->treeConfig);
			this->tabConfig->Controls->Add(this->groupBox1);
			this->tabConfig->Location = System::Drawing::Point(4, 22);
			this->tabConfig->Name = L"tabConfig";
			this->tabConfig->Padding = System::Windows::Forms::Padding(3);
			this->tabConfig->Size = System::Drawing::Size(406, 365);
			this->tabConfig->TabIndex = 3;
			this->tabConfig->Text = L"Configuration";
			this->tabConfig->UseVisualStyleBackColor = true;
			// 
			// label9
			// 
			this->label9->AutoSize = true;
			this->label9->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 12, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->label9->ForeColor = System::Drawing::Color::Navy;
			this->label9->Location = System::Drawing::Point(11, 3);
			this->label9->Name = L"label9";
			this->label9->Size = System::Drawing::Size(175, 24);
			this->label9->TabIndex = 13;
			this->label9->Text = L"Configuration Settings";
			this->label9->TextAlign = System::Drawing::ContentAlignment::MiddleCenter;
			this->label9->UseCompatibleTextRendering = true;
			// 
			// groupBox5
			// 
			this->groupBox5->Controls->Add(this->firmwareVersionBox2);
			this->groupBox5->Location = System::Drawing::Point(292, 30);
			this->groupBox5->Name = L"groupBox5";
			this->groupBox5->Size = System::Drawing::Size(111, 48);
			this->groupBox5->TabIndex = 12;
			this->groupBox5->TabStop = false;
			this->groupBox5->Text = L"Firmware Version";
			// 
			// firmwareVersionBox2
			// 
			this->firmwareVersionBox2->Location = System::Drawing::Point(18, 19);
			this->firmwareVersionBox2->Name = L"firmwareVersionBox2";
			this->firmwareVersionBox2->ReadOnly = true;
			this->firmwareVersionBox2->Size = System::Drawing::Size(70, 20);
			this->firmwareVersionBox2->TabIndex = 9;
			// 
			// treeConfig
			// 
			this->treeConfig->Location = System::Drawing::Point(0, 84);
			this->treeConfig->Name = L"treeConfig";
			this->treeConfig->Size = System::Drawing::Size(406, 281);
			this->treeConfig->TabIndex = 4;
			// 
			// groupBox1
			// 
			this->groupBox1->Controls->Add(this->flashWriteButton);
			this->groupBox1->Controls->Add(this->readButton);
			this->groupBox1->Controls->Add(this->writeButton);
			this->groupBox1->Location = System::Drawing::Point(3, 30);
			this->groupBox1->Name = L"groupBox1";
			this->groupBox1->Size = System::Drawing::Size(283, 48);
			this->groupBox1->TabIndex = 3;
			this->groupBox1->TabStop = false;
			this->groupBox1->Text = L"Communication";
			// 
			// flashWriteButton
			// 
			this->flashWriteButton->Location = System::Drawing::Point(178, 16);
			this->flashWriteButton->Name = L"flashWriteButton";
			this->flashWriteButton->Size = System::Drawing::Size(89, 23);
			this->flashWriteButton->TabIndex = 2;
			this->flashWriteButton->Text = L"FLASH Commit";
			this->flashWriteButton->UseVisualStyleBackColor = true;
			this->flashWriteButton->Click += gcnew System::EventHandler(this, &Form1::flashWriteButton_Click);
			// 
			// readButton
			// 
			this->readButton->Location = System::Drawing::Point(8, 16);
			this->readButton->Name = L"readButton";
			this->readButton->Size = System::Drawing::Size(75, 23);
			this->readButton->TabIndex = 0;
			this->readButton->Text = L"Read";
			this->readButton->UseVisualStyleBackColor = true;
			this->readButton->Click += gcnew System::EventHandler(this, &Form1::readButton_Click);
			// 
			// writeButton
			// 
			this->writeButton->Location = System::Drawing::Point(89, 16);
			this->writeButton->Name = L"writeButton";
			this->writeButton->Size = System::Drawing::Size(83, 23);
			this->writeButton->TabIndex = 1;
			this->writeButton->Text = L"RAM Commit";
			this->writeButton->UseVisualStyleBackColor = true;
			this->writeButton->Click += gcnew System::EventHandler(this, &Form1::writeButton_Click);
			// 
			// tabCalibration
			// 
			this->tabCalibration->Controls->Add(this->labelMagDataPoints);
			this->tabCalibration->Controls->Add(this->label12);
			this->tabCalibration->Controls->Add(this->buttonComputeCalibration);
			this->tabCalibration->Controls->Add(this->labelCalibrationStatus);
			this->tabCalibration->Controls->Add(this->label13);
			this->tabCalibration->Controls->Add(this->buttonResetDataCollection);
			this->tabCalibration->Controls->Add(this->buttonStopDataCollection);
			this->tabCalibration->Controls->Add(this->buttonStartDataCollection);
			this->tabCalibration->Controls->Add(this->groupBox6);
			this->tabCalibration->Location = System::Drawing::Point(4, 22);
			this->tabCalibration->Name = L"tabCalibration";
			this->tabCalibration->Padding = System::Windows::Forms::Padding(3);
			this->tabCalibration->Size = System::Drawing::Size(406, 365);
			this->tabCalibration->TabIndex = 4;
			this->tabCalibration->Text = L"Mag Calibration";
			this->tabCalibration->UseVisualStyleBackColor = true;
			// 
			// labelMagDataPoints
			// 
			this->labelMagDataPoints->AutoSize = true;
			this->labelMagDataPoints->ForeColor = System::Drawing::Color::Red;
			this->labelMagDataPoints->Location = System::Drawing::Point(147, 81);
			this->labelMagDataPoints->Name = L"labelMagDataPoints";
			this->labelMagDataPoints->Size = System::Drawing::Size(13, 13);
			this->labelMagDataPoints->TabIndex = 29;
			this->labelMagDataPoints->Text = L"0";
			// 
			// label12
			// 
			this->label12->AutoSize = true;
			this->label12->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 8.25F, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->label12->Location = System::Drawing::Point(7, 81);
			this->label12->Name = L"label12";
			this->label12->Size = System::Drawing::Size(134, 13);
			this->label12->TabIndex = 28;
			this->label12->Text = L"Collected Data Points:";
			// 
			// buttonComputeCalibration
			// 
			this->buttonComputeCalibration->Enabled = false;
			this->buttonComputeCalibration->Location = System::Drawing::Point(10, 107);
			this->buttonComputeCalibration->Name = L"buttonComputeCalibration";
			this->buttonComputeCalibration->Size = System::Drawing::Size(113, 23);
			this->buttonComputeCalibration->TabIndex = 27;
			this->buttonComputeCalibration->Text = L"Compute Calibration";
			this->buttonComputeCalibration->UseVisualStyleBackColor = true;
			this->buttonComputeCalibration->Click += gcnew System::EventHandler(this, &Form1::buttonComputeCalibration_Click);
			// 
			// labelCalibrationStatus
			// 
			this->labelCalibrationStatus->AutoSize = true;
			this->labelCalibrationStatus->Location = System::Drawing::Point(60, 59);
			this->labelCalibrationStatus->Name = L"labelCalibrationStatus";
			this->labelCalibrationStatus->Size = System::Drawing::Size(24, 13);
			this->labelCalibrationStatus->TabIndex = 26;
			this->labelCalibrationStatus->Text = L"Idle";
			// 
			// label13
			// 
			this->label13->AutoSize = true;
			this->label13->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 8.25F, System::Drawing::FontStyle::Bold, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->label13->Location = System::Drawing::Point(7, 59);
			this->label13->Name = L"label13";
			this->label13->Size = System::Drawing::Size(47, 13);
			this->label13->TabIndex = 25;
			this->label13->Text = L"Status:";
			// 
			// buttonResetDataCollection
			// 
			this->buttonResetDataCollection->Enabled = false;
			this->buttonResetDataCollection->Location = System::Drawing::Point(260, 18);
			this->buttonResetDataCollection->Name = L"buttonResetDataCollection";
			this->buttonResetDataCollection->Size = System::Drawing::Size(51, 23);
			this->buttonResetDataCollection->TabIndex = 23;
			this->buttonResetDataCollection->Text = L"Reset";
			this->buttonResetDataCollection->UseVisualStyleBackColor = true;
			this->buttonResetDataCollection->Click += gcnew System::EventHandler(this, &Form1::buttonResetDataCollection_Click);
			// 
			// buttonStopDataCollection
			// 
			this->buttonStopDataCollection->Enabled = false;
			this->buttonStopDataCollection->Location = System::Drawing::Point(134, 18);
			this->buttonStopDataCollection->Name = L"buttonStopDataCollection";
			this->buttonStopDataCollection->Size = System::Drawing::Size(115, 23);
			this->buttonStopDataCollection->TabIndex = 22;
			this->buttonStopDataCollection->Text = L"Stop Data Collection";
			this->buttonStopDataCollection->UseVisualStyleBackColor = true;
			this->buttonStopDataCollection->Click += gcnew System::EventHandler(this, &Form1::buttonStopDataCollection_Click);
			// 
			// buttonStartDataCollection
			// 
			this->buttonStartDataCollection->Location = System::Drawing::Point(8, 18);
			this->buttonStartDataCollection->Name = L"buttonStartDataCollection";
			this->buttonStartDataCollection->Size = System::Drawing::Size(115, 23);
			this->buttonStartDataCollection->TabIndex = 21;
			this->buttonStartDataCollection->Text = L"Start Data Collection";
			this->buttonStartDataCollection->UseVisualStyleBackColor = true;
			this->buttonStartDataCollection->Click += gcnew System::EventHandler(this, &Form1::buttonStartDataCollection_Click);
			// 
			// groupBox6
			// 
			this->groupBox6->Controls->Add(this->label14);
			this->groupBox6->Controls->Add(this->label15);
			this->groupBox6->Controls->Add(this->label16);
			this->groupBox6->Controls->Add(this->label17);
			this->groupBox6->Controls->Add(this->label18);
			this->groupBox6->Controls->Add(this->buttonWriteMagConfigToRAM);
			this->groupBox6->Controls->Add(this->magBiasZ);
			this->groupBox6->Controls->Add(this->magMatrix12);
			this->groupBox6->Controls->Add(this->magBiasY);
			this->groupBox6->Controls->Add(this->magMatrix00);
			this->groupBox6->Controls->Add(this->magBiasX);
			this->groupBox6->Controls->Add(this->magMatrix01);
			this->groupBox6->Controls->Add(this->magMatrix22);
			this->groupBox6->Controls->Add(this->magMatrix02);
			this->groupBox6->Controls->Add(this->magMatrix21);
			this->groupBox6->Controls->Add(this->magMatrix10);
			this->groupBox6->Controls->Add(this->magMatrix20);
			this->groupBox6->Controls->Add(this->magMatrix11);
			this->groupBox6->Location = System::Drawing::Point(8, 146);
			this->groupBox6->Name = L"groupBox6";
			this->groupBox6->Size = System::Drawing::Size(388, 176);
			this->groupBox6->TabIndex = 24;
			this->groupBox6->TabStop = false;
			this->groupBox6->Text = L"Calibration Settings";
			// 
			// label14
			// 
			this->label14->AutoSize = true;
			this->label14->Location = System::Drawing::Point(263, 111);
			this->label14->Name = L"label14";
			this->label14->Size = System::Drawing::Size(14, 13);
			this->label14->TabIndex = 20;
			this->label14->Text = L"Z";
			// 
			// label15
			// 
			this->label15->AutoSize = true;
			this->label15->Location = System::Drawing::Point(263, 82);
			this->label15->Name = L"label15";
			this->label15->Size = System::Drawing::Size(14, 13);
			this->label15->TabIndex = 19;
			this->label15->Text = L"Y";
			// 
			// label16
			// 
			this->label16->AutoSize = true;
			this->label16->Location = System::Drawing::Point(263, 56);
			this->label16->Name = L"label16";
			this->label16->Size = System::Drawing::Size(14, 13);
			this->label16->TabIndex = 18;
			this->label16->Text = L"X";
			// 
			// label17
			// 
			this->label17->AutoSize = true;
			this->label17->Location = System::Drawing::Point(280, 30);
			this->label17->Name = L"label17";
			this->label17->Size = System::Drawing::Size(38, 13);
			this->label17->TabIndex = 17;
			this->label17->Text = L"Biases";
			// 
			// label18
			// 
			this->label18->AutoSize = true;
			this->label18->Location = System::Drawing::Point(12, 30);
			this->label18->Name = L"label18";
			this->label18->Size = System::Drawing::Size(87, 13);
			this->label18->TabIndex = 16;
			this->label18->Text = L"Calibration Matrix";
			// 
			// buttonWriteMagConfigToRAM
			// 
			this->buttonWriteMagConfigToRAM->Enabled = false;
			this->buttonWriteMagConfigToRAM->Location = System::Drawing::Point(14, 143);
			this->buttonWriteMagConfigToRAM->Name = L"buttonWriteMagConfigToRAM";
			this->buttonWriteMagConfigToRAM->Size = System::Drawing::Size(118, 23);
			this->buttonWriteMagConfigToRAM->TabIndex = 15;
			this->buttonWriteMagConfigToRAM->Text = L"Write to RAM";
			this->buttonWriteMagConfigToRAM->UseVisualStyleBackColor = true;
			this->buttonWriteMagConfigToRAM->Click += gcnew System::EventHandler(this, &Form1::buttonWriteMagConfigToRAM_Click);
			// 
			// magBiasZ
			// 
			this->magBiasZ->Location = System::Drawing::Point(283, 108);
			this->magBiasZ->Name = L"magBiasZ";
			this->magBiasZ->Size = System::Drawing::Size(85, 20);
			this->magBiasZ->TabIndex = 14;
			// 
			// magMatrix12
			// 
			this->magMatrix12->Location = System::Drawing::Point(169, 82);
			this->magMatrix12->Name = L"magMatrix12";
			this->magMatrix12->Size = System::Drawing::Size(71, 20);
			this->magMatrix12->TabIndex = 8;
			// 
			// magBiasY
			// 
			this->magBiasY->Location = System::Drawing::Point(283, 79);
			this->magBiasY->Name = L"magBiasY";
			this->magBiasY->Size = System::Drawing::Size(85, 20);
			this->magBiasY->TabIndex = 13;
			// 
			// magMatrix00
			// 
			this->magMatrix00->Location = System::Drawing::Point(15, 56);
			this->magMatrix00->Name = L"magMatrix00";
			this->magMatrix00->Size = System::Drawing::Size(71, 20);
			this->magMatrix00->TabIndex = 3;
			// 
			// magBiasX
			// 
			this->magBiasX->Location = System::Drawing::Point(283, 53);
			this->magBiasX->Name = L"magBiasX";
			this->magBiasX->Size = System::Drawing::Size(85, 20);
			this->magBiasX->TabIndex = 12;
			// 
			// magMatrix01
			// 
			this->magMatrix01->Location = System::Drawing::Point(92, 56);
			this->magMatrix01->Name = L"magMatrix01";
			this->magMatrix01->Size = System::Drawing::Size(71, 20);
			this->magMatrix01->TabIndex = 4;
			// 
			// magMatrix22
			// 
			this->magMatrix22->Location = System::Drawing::Point(169, 108);
			this->magMatrix22->Name = L"magMatrix22";
			this->magMatrix22->Size = System::Drawing::Size(71, 20);
			this->magMatrix22->TabIndex = 11;
			// 
			// magMatrix02
			// 
			this->magMatrix02->Location = System::Drawing::Point(169, 56);
			this->magMatrix02->Name = L"magMatrix02";
			this->magMatrix02->Size = System::Drawing::Size(71, 20);
			this->magMatrix02->TabIndex = 5;
			// 
			// magMatrix21
			// 
			this->magMatrix21->Location = System::Drawing::Point(92, 108);
			this->magMatrix21->Name = L"magMatrix21";
			this->magMatrix21->Size = System::Drawing::Size(71, 20);
			this->magMatrix21->TabIndex = 10;
			// 
			// magMatrix10
			// 
			this->magMatrix10->Location = System::Drawing::Point(15, 82);
			this->magMatrix10->Name = L"magMatrix10";
			this->magMatrix10->Size = System::Drawing::Size(71, 20);
			this->magMatrix10->TabIndex = 6;
			// 
			// magMatrix20
			// 
			this->magMatrix20->Location = System::Drawing::Point(15, 108);
			this->magMatrix20->Name = L"magMatrix20";
			this->magMatrix20->Size = System::Drawing::Size(71, 20);
			this->magMatrix20->TabIndex = 9;
			// 
			// magMatrix11
			// 
			this->magMatrix11->Location = System::Drawing::Point(92, 82);
			this->magMatrix11->Name = L"magMatrix11";
			this->magMatrix11->Size = System::Drawing::Size(71, 20);
			this->magMatrix11->TabIndex = 7;
			// 
			// statusBox
			// 
			this->statusBox->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 10, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point, 
				static_cast<System::Byte>(0)));
			this->statusBox->Location = System::Drawing::Point(0, 393);
			this->statusBox->Name = L"statusBox";
			this->statusBox->ReadOnly = true;
			this->statusBox->Size = System::Drawing::Size(410, 155);
			this->statusBox->TabIndex = 1;
			this->statusBox->Text = L"";
			// 
			// packetResponseTimer
			// 
			this->packetResponseTimer->Interval = 500;
			this->packetResponseTimer->Tick += gcnew System::EventHandler(this, &Form1::packetResponseTimer_Tick);
			// 
			// openFileDialog1
			// 
			this->openFileDialog1->Filter = L".hex files|*.hex";
			this->openFileDialog1->Title = L"Select Firmware File";
			// 
			// Form1
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(413, 550);
			this->Controls->Add(this->statusBox);
			this->Controls->Add(this->tabControl);
			this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedSingle;
			this->Icon = (cli::safe_cast<System::Drawing::Icon^  >(resources->GetObject(L"$this.Icon")));
			this->MaximizeBox = false;
			this->MinimizeBox = false;
			this->Name = L"Form1";
			this->Text = L"CHRobotics Serial Interface v1.2.1 - Disconnected";
			this->tabControl->ResumeLayout(false);
			this->tabSerialSetting->ResumeLayout(false);
			this->tabSerialSetting->PerformLayout();
			this->groupBox3->ResumeLayout(false);
			this->groupBox3->PerformLayout();
			this->groupBox2->ResumeLayout(false);
			this->groupBox4->ResumeLayout(false);
			this->groupBox4->PerformLayout();
			this->groupBox7->ResumeLayout(false);
			this->groupBox7->PerformLayout();
			this->tabCommands->ResumeLayout(false);
			this->tabCommands->PerformLayout();
			this->tabData->ResumeLayout(false);
			this->tabData->PerformLayout();
			this->tabConfig->ResumeLayout(false);
			this->tabConfig->PerformLayout();
			this->groupBox5->ResumeLayout(false);
			this->groupBox5->PerformLayout();
			this->groupBox1->ResumeLayout(false);
			this->tabCalibration->ResumeLayout(false);
			this->tabCalibration->PerformLayout();
			this->groupBox6->ResumeLayout(false);
			this->groupBox6->PerformLayout();
			this->ResumeLayout(false);

		}
#pragma endregion

		private:

		void SerialPacketError_eventHandler( String^ text )
		{
			this->addStatusTextSafe(text, Color::Red);
		}
		
		void SerialPacketReceived_eventHandler( SerialPacket^ packet )
		{
			// Make note of new packet reception (this handles the packet timer, so that if a packet has been sent
			// and a response is received, the timer is stopped)
			PacketReceived( packet );
			
			// If no firmware definition has been selected yet, ignore this packet if it isn't reporting the firmware version
			if( this->SelectedFirmwareIndex == -1 )
			{
				if( packet->Address != UM6_GET_FW_VERSION )
				{
					return;
				}
			}

			// Check if the received packet is a firmware defintion
			if( packet->Address == UM6_GET_FW_VERSION )
			{
				cli::array<wchar_t,1>^ data = gcnew cli::array<wchar_t,1>(4);

				data[0] = packet->GetDataByte(0);
				data[1] = packet->GetDataByte(1);
				data[2] = packet->GetDataByte(2);
				data[3] = packet->GetDataByte(3);

				String^ PacketFirmwareID = gcnew String(data);

				// This is a firmware definition packet.  Search loaded firmware items to determine if we have a definition
				// for the firmware revision given by the sensor.
				for( int i = 0; i < this->FirmwareCount; i++ )
				{
					String^ FirmwareID = FirmwareArray[i]->GetID();
					
					if( FirmwareID == PacketFirmwareID )
					{
						this->SelectedFirmwareIndex = i;
						updateFWTextSafe( FirmwareID );

						break;
					}									
				}

				this->addStatusTextSafe(L"Received packet ID message from sensor.", Color::Green);

				if( SelectedFirmwareIndex == -1 )
				{
					this->addStatusTextSafe("Error: Could not find firmware definition for attached device.", Color::Red);
				}
				else
				{
					addDataItemsSafe();
					updateGraphButtonSafe( true );
					addCommandsSafe();
				}
			}
			else if( packet->Address == UM6_BAD_CHECKSUM )
			{
//				this->addStatusTextSafe(L"Received BAD_CHECKSUM message from sensor.", Color::Red);
			}
			else if( packet->Address == UM6_UNKNOWN_ADDRESS )
			{
				this->addStatusTextSafe(L"Received UNKNOWN_ADDRESS message from sensor.", Color::Red);
			}
			else if( packet->Address == UM6_INVALID_BATCH_SIZE )
			{
				this->addStatusTextSafe(L"Received INVALID_BATCH_SIZE message from sensor.", Color::Red);
			}
			else if( packet->HasData )
			{
				// Packet has data.  Copy data into local registers.
				unsigned char regIndex = 0;
				for( int i = 0; i < packet->DataLength; i += 4 )
				{
					UInt32 data = (packet->GetDataByte(i) << 24) | (packet->GetDataByte(i+1) << 16) | (packet->GetDataByte(i+2) << 8) | (packet->GetDataByte(i+3));
					FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( packet->Address + regIndex, data );
					regIndex++;
				}

				// If this packet reported the contents of data registers, update local data registers accordingly
				if( (packet->Address >= DATA_REGISTER_START_ADDRESS) && (packet->Address < COMMAND_START_ADDRESS) )
				{
					// Copy new register data into individual items for display in GUI
					updateItemsSafe( DATA_UPDATE );	

					// If magnetometer data collection (for calibration) is enabled, check to determine if this packet contained raw
					// magnetometer data.  This code is device-specific, and it would be preferable to find another way to do this
					// since the rest of this interface software makes no assumptions about the formatting of the registers aboard
					// the device... Anyway, this code should work for any device with ID UM1*, where * can be anything.
					String^ firmware_id = FirmwareArray[SelectedFirmwareIndex]->GetID();
					if( firmware_id->Substring(0,2)  == "UM" )
					{
						// Is data collection enabled?
						if( this->magDataCollectionEnabled )
						{
							// Check to see if this is raw mag. data (register address of first raw mag register is 90.  Second is 91.
							if( packet->IsBatch && (packet->BatchLength == 2) && (packet->Address == 90) )
							{
								// New raw mag data has arrived.  Write it to the mag logging array.
								Int16 mag_x = (packet->GetDataByte(0) << 8) | (packet->GetDataByte(1));
								Int16 mag_y = (packet->GetDataByte(2) << 8) | packet->GetDataByte(3);
								Int16 mag_z = (packet->GetDataByte(4) << 8) | packet->GetDataByte(5);

								// Make sure we don't read too much data
								if( this->rawMagDataPointer < MAXIMUM_MAG_DATA_POINTS )
								{
									this->rawMagData[this->rawMagDataPointer,0] = (double)mag_x;
									this->rawMagData[this->rawMagDataPointer,1] = (double)mag_y;
									this->rawMagData[this->rawMagDataPointer,2] = (double)mag_z;

									this->rawMagDataPointer++;

									updateMagCounterLabelSafe();
								}
							}
						}
					}
				}
				// If this packet reported the contents of configuration registers, update local configuration registers accordingly
				else if( packet->Address < DATA_REGISTER_START_ADDRESS )
				{
//					FirmwareRegister^ current_register = FirmwareArray[SelectedFirmwareIndex]->GetRegister(packet->Address);
					updateItemsSafe( CONFIG_UPDATE );
//					this->addStatusTextSafe(L"Received " + current_register->Name + " register contents.", Color::Green);
				}
				// This should never be reached for a properly formatted packet (ie. commands should never contain data)
				else
				{
					
				}
			}
			// Packet has not data.  If a configuration register address, the packet signifies that a write operation
			// to a configuration register was just completed.
			else if( packet->Address < DATA_REGISTER_START_ADDRESS )
			{
				FirmwareRegister^ current_register = FirmwareArray[SelectedFirmwareIndex]->GetRegister(packet->Address);
				current_register->UserModified = false;
				this->addStatusTextSafe(L"Successfully wrote to " + current_register->Name + " register.", Color::Green);
				this->resetTreeNodeColorSafe( packet->Address );
			}
			// Packet has no data.  If a command register address, the packet signals that a received command either succeeded or
			// failed.
			else if( packet->Address >= COMMAND_START_ADDRESS )
			{
				String^ command_name = FirmwareArray[SelectedFirmwareIndex]->GetCommandName( packet->Address );

				// Check to see if command succeeded
				if( packet->CommandFailed == 1 )
				{					
					this->addStatusTextSafe(L"Command failed: " + command_name, Color::Green);
				}
				else
				{
					this->addStatusTextSafe(L"Command complete: " + command_name, Color::Red);
				}
			}
		}

		void resetTreeNodeColorSafe( unsigned char register_address )
		{
			if( this->InvokeRequired )
			{
				cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
				args[0] = register_address;
				this->BeginInvoke( gcnew UpdateFormUCharDelegate( this, &Form1::resetTreeNodeColorSafe ), args );
			}
			else
			{
				// Iterate through all configuration items.  For each item that is affected by the register with register_address,
				// set the background color to transparent.  The motivation here is that when an item has been modified, it's
				// background color changes to make it obvious that the data has been changed and needs to be written to the sensor.
				// Once it has been written, the background color needs to be changed back.
				int config_item_count = FirmwareArray[SelectedFirmwareIndex]->GetConfigItemCount();

				for( int i = 0; i < config_item_count; i++ )
				{
					FirmwareItem^ current_item = FirmwareArray[SelectedFirmwareIndex]->GetConfigItem(i);

					if( current_item->GetAddress() == register_address )
					{
						current_item->BackColor = Color::Transparent;
					}
				}
			}
		}

		void addConfigTreeNodesSafe()
		{
			if( this->InvokeRequired )
			{
				this->BeginInvoke( gcnew UpdateFormDelegate( this, &Form1::addConfigTreeNodesSafe ) );
			}
			else
			{
				treeConfig->Nodes->Clear();

				for( UInt32 i = 0; i < FirmwareArray[SelectedFirmwareIndex]->GetConfigItemCount(); i++ )
				{
					FirmwareItem^ current_item = FirmwareArray[SelectedFirmwareIndex]->GetConfigItem( i );
					current_item->BackColor = Color::Transparent;
					if( current_item->GetParentIndex() == i )
					{
						treeConfig->Nodes->Add(current_item);
					}
				}
			}
		}

		void addCommandsSafe()
		{
			if( this->InvokeRequired )
			{
				this->BeginInvoke( gcnew UpdateFormDelegate( this, &Form1::addCommandsSafe ) );
			}
			else
			{
				this->commandsListBox->Items->Clear();

				for( UInt32 i = 0; i < this->FirmwareArray[SelectedFirmwareIndex]->GetCommandCount(); i++ )
				{
					FirmwareCommand^ command = this->FirmwareArray[SelectedFirmwareIndex]->GetCommand( i );
					this->commandsListBox->Items->Add( command->Name );
				}
			}
		}


		void updateGraphButtonSafe( bool value )
		{
			if( this->InvokeRequired )
			{
				cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
				args[0] = value;
				this->BeginInvoke( gcnew UpdateFormBoolDelegate( this, &Form1::updateGraphButtonSafe ), args );
			}
			else
			{
				this->dataGraphButton->Enabled = value;
			}
		}


		void updateItemsSafe( int update_type )
		{
			if( SelectedFirmwareIndex == -1 )
			{
				return;
			}

			if( this->InvokeRequired )
			{
				cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
				args[0] = update_type;
				this->BeginInvoke( gcnew UpdateFormIntDelegate( this, &Form1::updateItemsSafe ), args );
			}
			else
			{
				FirmwareArray[SelectedFirmwareIndex]->UpdateItemsFromRegisters( update_type );
			}
		}

		void updateFWTextSafe( String^ text )
		{
			if( this->InvokeRequired )
			{
				cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
				args[0] = text;
				this->BeginInvoke( gcnew UpdateFormTextDelegate( this, &Form1::updateFWTextSafe ),  args);
			}
			else
			{
				this->firmwareVersionBox->Text = text;
				this->firmwareVersionBox2->Text = text;
			}
		}

		void addStatusTextSafe( String^ text, System::Drawing::Color color )
		{
			if( this->InvokeRequired )
			{
				cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(2);
				args[0] = text;
				args[1] = color;

				this->BeginInvoke( gcnew UpdateStatusDelegate( this, &Form1::addStatusTextSafe ),  args);
			}
			else
			{
				int selection_start = this->statusBox->TextLength;

				this->statusBox->Text += (text + L"\r\n");
				/*
				this->statusBox->SelectionLength = this->statusBox->TextLength - selection_start;				
				this->statusBox->SelectionStart = selection_start;
				this->statusBox->SelectionColor = color;
				*/

				this->statusBox->SelectionStart = 0;
				this->statusBox->SelectionLength = this->statusBox->TextLength;
				this->statusBox->SelectionBullet = true;
				this->statusBox->SelectionHangingIndent = 15;

				this->statusBox->Focus();
				this->statusBox->SelectionStart = this->statusBox->TextLength;
				this->statusBox->ScrollToCaret();
			}
		}

		void addDataItemsSafe()
		{
			if( SelectedFirmwareIndex == -1 )
				return;

			if( this->InvokeRequired )
			{
				this->BeginInvoke( gcnew UpdateFormDelegate( this, &Form1::addDataItemsSafe ) );
			}
			else
			{
				// Clear any existing nodes in data box
				treeData->Nodes->Clear();

				// Add nodes to data box
				for( UInt32 i = 0; i < FirmwareArray[SelectedFirmwareIndex]->GetDataItemCount(); i++ )
				{
					FirmwareItem^ current_item = FirmwareArray[SelectedFirmwareIndex]->GetDataItem( i );
					if( current_item->GetParentIndex() == i )
					{
						treeData->Nodes->Add(current_item);
					}
				}
			}
		}

		// Function for updating the text that indicates how many raw magnetometer data points have been collected for calibration
		void updateMagCounterLabelSafe()
		{
			if( this->InvokeRequired )
			{
				this->BeginInvoke( gcnew UpdateFormDelegate( this, &Form1::updateMagCounterLabelSafe ) );
			}
			else
			{
				this->labelMagDataPoints->Text = this->rawMagDataPointer.ToString();

				if( this->rawMagDataPointer > 400 )
				{
					this->labelMagDataPoints->ForeColor = Color::Green;
					this->buttonComputeCalibration->Enabled = true;
				}
				else
				{
					this->labelMagDataPoints->ForeColor = Color::Red;
					this->buttonComputeCalibration->Enabled = false;
				}
			}
		}

		// Function for updating the magnetometer calibration status label
		void updateMagStatusLabelSafe( String^ text )
		{
			if( this->InvokeRequired )
			{
				cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
				args[0] = text;
				this->BeginInvoke( gcnew UpdateFormTextDelegate( this, &Form1::updateMagStatusLabelSafe ),  args);
			}
			else
			{
				this->labelCalibrationStatus->Text = text;
			}
		}

#pragma region Tree Node Editing Functions
		System::Void updateNodeData( int item_index )
		{
			if( item_index == -1 )
			{
				return;
			}

			FirmwareItem^ node = FirmwareArray[SelectedFirmwareIndex]->GetConfigItem( item_index );

			String^ DataType = node->GetDataType();
			UInt32 mask = (UInt32)System::Math::Pow(2,node->GetBits()) - 1;
			bool data_changed = false;

			// If this item is a binary or en/dis input, then it was already updated when the combo box selection was changed
			if( DataType == L"en/dis" || DataType == L"binary" )
			{
				return;
			}

			// If datatype was a float, attempt to parse the new data.  On failure, reset node data to what was stored in the register
			// and write a message to the status box to inform the user of the problem
			if( DataType == L"float" )
			{
				try
				{
					float new_data = float::Parse( configTextBox->Text );
					if( new_data != node->GetFloatData() )
					{
						node->SetFloatData( new_data );
						data_changed = true;
					}
				}
				catch( Exception^ /*e*/ )
				{
					statusBox->Text = statusBox->Text + "ERROR: Unable to write data to register.  Data must be representable with floating point value.\r\n";
					FirmwareArray[SelectedFirmwareIndex]->UpdateItemFromRegister( item_index, CONFIG_ITEM );

					return;
				}
			}
			else if( DataType == L"int16" )
			{
				try
				{
					Int32 new_data = (Int32)(Int16::Parse( configTextBox->Text ) );
					if( new_data != node->GetIntData() )
					{
						node->SetIntData( new_data );
						data_changed = true;
					}
				}
				catch( Exception^ /*e*/ )
				{
					statusBox->Text = statusBox->Text + "ERROR: Unable to write data to register.  Data must be representable with 16-bit signed integer.\r\n";
					FirmwareArray[SelectedFirmwareIndex]->UpdateItemFromRegister( item_index, CONFIG_ITEM );

					return;
				}
			}
			else if( DataType == L"uint16" )
			{
				try
				{
					UInt32 new_data = (UInt32)(UInt16::Parse( configTextBox->Text ) & mask);
					if( new_data != node->GetUIntData() )
					{
						node->SetUIntData( new_data );
						data_changed = true;
					}
					
				}
				catch( Exception^ /*e*/ )
				{
					statusBox->Text = statusBox->Text + "ERROR: Unable to write data to register.  Data must be representable with 16-bit unsigned integer.\r\n";
					FirmwareArray[SelectedFirmwareIndex]->UpdateItemFromRegister( item_index, CONFIG_ITEM );

					return;
				}
			}
			else if( DataType == L"int32" )
			{
				try
				{
					Int32 new_data = (Int32)(Int32::Parse( configTextBox->Text ));
					if( new_data != node->GetIntData() )
					{
						node->SetIntData( new_data );
						data_changed = true;
					}
				}
				catch( Exception^ /*e*/ )
				{
					statusBox->Text = statusBox->Text + "ERROR: Unable to write data to register.  Data must be representable with 32-bit signed integer.\r\n";
					FirmwareArray[SelectedFirmwareIndex]->UpdateItemFromRegister( item_index, CONFIG_ITEM );

					return;
				}
			}
			else if( DataType == L"uint32" )
			{
				try
				{
					Int32 new_data = (UInt32)(UInt32::Parse( configTextBox->Text ) & mask);
					if( new_data != node->GetUIntData() )
					{
						node->SetUIntData( new_data );
						data_changed = true;
					}
				}
				catch( Exception^ /*e*/ )
				{
					statusBox->Text = statusBox->Text + "ERROR: Unable to write data to register.  Data must be representable with 32-bit unsigned integer.\r\n";
					FirmwareArray[SelectedFirmwareIndex]->UpdateItemFromRegister( item_index, CONFIG_ITEM );

					return;
				}
			}

			if( data_changed )
			{
				node->BackColor = Color::LightBlue;
				node->SetTitle();
				FirmwareArray[SelectedFirmwareIndex]->UpdateRegisterFromItem( item_index, CONFIG_ITEM );
			}
		}



		System::Void firmwareBox_NodeClicked(Object^ sender, TreeNodeMouseClickEventArgs^ e) 
		{
			FirmwareItem^ node;
			node = dynamic_cast<FirmwareItem^> (e->Node);

			String^ dataType = node->GetDataType();

			TreeView^ parent = dynamic_cast<TreeView^> (sender);

			this->configTextBox->Visible = false;
			this->configComboBox->Visible = false;

			// Take the data entered into the current selected item and save to registers for future writing
			updateNodeData( SelectedItemIndex );

			// Check the newly selected node.  If it is a heading, turn off all input controls and return
			if( node->GetParentIndex() == UInt32::Parse(node->Name) )
			{
				SelectedItemIndex = -1;
				return;
			}

			// Set the new item index to correspond with the newly selectd node
			SelectedItemIndex = int::Parse(node->Name);

			this->SelectedItemIndex = int::Parse( node->Name );

			// use the location of the selected node to create an input box next to the node's position
			Rectangle node_bounds = node->Bounds;
			Point tree_location = node->TreeView->Location;
			int x_loc = node_bounds.Width + node_bounds.X + 10;
			int y_loc = node_bounds.Y - 2;

			this->SuspendLayout();
			
			if( dataType == L"float" || dataType == L"int16" || dataType == L"string" || dataType == L"uint16" || dataType == L"int23" || dataType == L"uint32" )
			{
				this->configTextBox->Location = System::Drawing::Point(x_loc, y_loc);
				this->configTextBox->Name = L"configTextBox";
				this->configTextBox->Size = System::Drawing::Size(100, node_bounds.Height);
				
				this->configTextBox->Text = node->GetStringData();

				this->configTextBox->Visible = true;
				this->configTextBox->Enabled = true;
				this->configTextBox->BringToFront();
				this->configTextBox->SelectAll();
			}
			else if( dataType == L"binary" )
			{
				this->configComboBox->Location = System::Drawing::Point(x_loc, y_loc);
				this->configComboBox->Name = L"configComboBox";
				this->configComboBox->Size = System::Drawing::Size(40, node_bounds.Height);

				configComboBox->Items->Clear();
				configComboBox->Items->Add( L"0" );
				configComboBox->Items->Add( L"1" );
				
				if( node->GetBinaryData() )
				{
					configComboBox->SelectedIndex = 1;
				}
				else
				{
					configComboBox->SelectedIndex = 0;
				}

				this->configComboBox->Visible = true;
				this->configComboBox->Enabled = true;
				this->configComboBox->BringToFront();
			}
			else if( dataType == L"en/dis" )
			{
				this->configComboBox->Location = System::Drawing::Point(x_loc, y_loc);
				this->configComboBox->Name = L"configComboBox";
				this->configComboBox->Size = System::Drawing::Size(100, node_bounds.Height);

				configComboBox->Items->Clear();
				configComboBox->Items->Add( L"0 - Disabled" );
				configComboBox->Items->Add( L"1 - Enabled" );
				
				if( node->GetBinaryData() )
				{
					configComboBox->SelectedIndex = 1;
				}
				else
				{
					configComboBox->SelectedIndex = 0;
				}

				this->configComboBox->Visible = true;
				this->configComboBox->Enabled = true;
				this->configComboBox->BringToFront();
			}
			else if( dataType == L"option" )
			{
				this->configComboBox->Location = System::Drawing::Point(x_loc, y_loc);
				this->configComboBox->Name = L"configComboBox";
				this->configComboBox->Size = System::Drawing::Size(100, node_bounds.Height);

				configComboBox->Items->Clear();
				UInt32 optionCount = node->GetOptionCount();
				for( UInt32 i = 0; i < optionCount; i++ )
				{
					FirmwareOption^ option = node->GetFirmwareOption(i);
					configComboBox->Items->Add( option->Name );
				}

				this->configComboBox->SelectedIndex = node->GetOptionSelection();

				this->configComboBox->Visible = true;
				this->configComboBox->Enabled = true;
				this->configComboBox->BringToFront();
			}

			this->ResumeLayout();
		}


		System::Void firmwareBox_LostFocus( Object^ sender, EventArgs^ e )
		{
			if( this->configTextBox->Focused || this->configComboBox->Focused )
			{
				return;
			}
			else
			{
				this->configTextBox->Visible = false;
				this->configComboBox->Visible = false;

				updateNodeData( SelectedItemIndex );
			}
		}

		System::Void treeBox_Invalidated( Object^ sender, InvalidateEventArgs^ e )
		{

		}

		System::Void firmwareBox_MouseWheel( Object^ sender, MouseEventArgs^ e )
		{
			this->configTextBox->Visible = false;
			this->configComboBox->Visible = false;

			updateNodeData( SelectedItemIndex );
		}

		System::Void firmwareBox_Altered( Object^ sender, TreeViewEventArgs^ e )
		{
			this->configTextBox->Visible = false;
			this->configComboBox->Visible = false;
		}

		System::Void configCombo_SelectionChanged( Object^ sender, EventArgs^ e )
		{
			FirmwareItem^ node;

			// Make sure an item is selected
			if( SelectedItemIndex == -1 )
			{
				return;
			}

			// Use the selected item index to identify the firmware item being edited
			node = FirmwareArray[SelectedFirmwareIndex]->GetConfigItem( SelectedItemIndex );
			bool data_changed = false;

			// Change the data specified by the selection
			if( node->GetDataType() == L"en/dis" || node->GetDataType() == L"binary" )
			{
				if( configComboBox->SelectedIndex == 1 )
				{
					if( node->GetBinaryData() == false )
					{
						node->SetBinaryData( true );
						data_changed = true;
					}
				}
				else
				{
					if( node->GetBinaryData() == true )
					{
						node->SetBinaryData( false );
						data_changed = true;
					}
				}
			}
			else if( node->GetDataType() == L"option" )
			{
				if( node->GetOptionSelection() != configComboBox->SelectedIndex )
				{
					node->SetOptionSelection( configComboBox->SelectedIndex );
					data_changed = true;
				}
			}

			if( data_changed )
			{
				FirmwareArray[SelectedFirmwareIndex]->UpdateRegisterFromItem( SelectedItemIndex, CONFIG_ITEM );
				node->SetTitle();
				node->BackColor = Color::LightBlue;
			}
		}
#pragma endregion

	private: System::Void tabSerialSetting_Click(System::Object^  sender, System::EventArgs^  e) {
			 }

		 // Attempt to connect to serial port with specified settings
private: System::Void serialConnectButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 // Copy settings to serial port
			 serialConnector->BaudRate = int::Parse(dynamic_cast<String^>(this->serialBaudComboBox->SelectedItem));
			 
			 String^ ParityText = dynamic_cast<String^>(this->serialParityComboBox->SelectedItem);
			 if( ParityText == L"Odd" )
				serialConnector->Parity = System::IO::Ports::Parity::Odd;
			 else if( ParityText == L"Even" )
				 serialConnector->Parity = System::IO::Ports::Parity::Even;
			 else if( ParityText == L"None" )
				 serialConnector->Parity = System::IO::Ports::Parity::None;
			 else if( ParityText == L"Mark" )
				 serialConnector->Parity = System::IO::Ports::Parity::Mark;
			 else if( ParityText == L"Space" )
				 serialConnector->Parity = System::IO::Ports::Parity::Space;


			 serialConnector->PortName = dynamic_cast<String^>(this->serialComboBox->SelectedItem);

			 String^ StopBits = dynamic_cast<String^>(this->serialStopBitsComboBox->SelectedItem);
			 if( StopBits == L"1" )
				 serialConnector->StopBits = System::IO::Ports::StopBits::One;
			 else if( StopBits == L"2" )
				 serialConnector->StopBits = System::IO::Ports::StopBits::Two;

			 // Attemp to connect
			 try
			 {
				 serialConnector->Open();
			 }
			 catch( Exception^ /*e*/ )
			 {
				 this->addStatusTextSafe(L"Error: Could not open serial port.  Is it in use by another application?", Color::Red);

				 return;
			 }

			 this->Text = "CH Robotics Interface v" + version + " - Connected (" + dynamic_cast<String^>(this->serialComboBox->SelectedItem) + ")";
			 this->serialConnectButton->Enabled = false;
			 this->serialDisconnectButton->Enabled = true;
			 this->getFirmwareVersionButton->Enabled = true;
			 this->readButton->Enabled = true;
			 this->writeButton->Enabled = true;
			 this->flashWriteButton->Enabled = true;

			 SerialPacket^ fwRequest = gcnew SerialPacket();
			 fwRequest->HasData = false;
			 fwRequest->IsBatch = false;
			 fwRequest->Address = UM6_GET_FW_VERSION;

			 this->AddTXPacket( fwRequest );
		 }

		 /* *****************************************************************************
		 * Name: AddTXPacket
		 * Description: 
			Adds the current packet to the queue to be transmitted as soon as a
			possible
		 ** ****************************************************************************/
private: void AddTXPacket( SerialPacket^ packet )
		 {
			 this->packetsToSend[this->packetCount] = packet;
			 this->packetCount++;
			 
			 // Check to see if the packet timer is running.  If it is, then a packet has been
			 // transmitted and we are waiting for a response.
			 // If the timer is not running, then start it and transmit the first packet in the
			 // buffer
			 if( !this->packetResponseTimer->Enabled )
			 {
				 serialConnector->TransmitPacket( packetsToSend[0] );
				 this->packetResponseTimer->Start();
			 }
		 }

		 /* *****************************************************************************
		 * Name: PacketReceived
		 * Description: 
			Checks the received packet to determine if it was waiting for it.  If it was,
			remove the packet from the packetsToSend queue.  If it wasn't, then ignore it.
		 ** ****************************************************************************/
private: void PacketReceived( SerialPacket^ packet )
		 {
			 if( this->InvokeRequired )
			 {
				 cli::array<System::Object^>^ args = gcnew cli::array<System::Object^>(1);
				 args[0] = packet;

				 this->BeginInvoke( gcnew UpdateFormSerialPacketDelegate( this, &Form1::PacketReceived ),  args);
			 }
			 else
			 {

				 if( this->packetCount == 0 )
				 {
					 return;
				 }

				 if( packet->Address == packetsToSend[0]->Address )
				 {
					// Stop the packet wait timer
					this->packetResponseTimer->Stop();

					// Shift other packets in the buffer down.
					for( int i = 1; i < this->packetCount; i++ )
					{
						packetsToSend[i-1] = packetsToSend[i];
					}

					this->packetCount--;
					this->packetRetryCount = 0;

					// If there are still packets to send, then start the timer and send the next one
					if( packetCount > 0 )
					{
						serialConnector->TransmitPacket( packetsToSend[0] );
						this->packetResponseTimer->Start();
					}

					if( this->readingConfigData )
					{
						this->registersRead++;
						this->configProgress->SetProgress( registersRead );
					}
				 }

				 // If all configuration registers were being read and there are no more packets to transmit,
				 // then all data has been read.  Populate configuration tree view.
				 if( (this->readingConfigData) && (this->packetCount == 0) )
				 {
					 this->addConfigTreeNodesSafe();

					 this->readingConfigData = false;

					 this->addStatusTextSafe( L"Finished reading configuration settings from device.", Color::Green);
				 }

			 }
		 }

		 // Disconnect from serial port
private: System::Void serialDisconnectButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->SerialDisconnect();			 
		 }

private: System::Void SerialDisconnect()
		 {
			 this->Text = "CH Robotics Interface v" + version + " - Disconnected";
			 this->serialConnector->Close();
			 this->serialDisconnectButton->Enabled = false;
			 this->serialConnectButton->Enabled = true;
			 this->getFirmwareVersionButton->Enabled = false;

			 this->readButton->Enabled = false;
			 this->writeButton->Enabled = false;
			 this->flashWriteButton->Enabled = false;

			 this->treeData->Nodes->Clear();
			 this->treeConfig->Nodes->Clear();
			 this->commandsListBox->Items->Clear();

			 this->SelectedFirmwareIndex = -1;
			 this->updateFWTextSafe(L"");

			 this->dataGraphButton->Enabled = false;
		 }

		 // Read configuration register values from device
private: System::Void readButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 if( this->SelectedFirmwareIndex == -1 )
				 return;

			 this->packetResponseTimer->Stop();
			 this->packetCount = 0;

			 // Iterate through all possible configuration register addresses and send packets requesting
			 // data from registers that exist
			 for( int i = 0; i < DATA_REGISTER_START_ADDRESS; i++ )
			 {
				 FirmwareRegister^ current_register = this->FirmwareArray[SelectedFirmwareIndex]->GetRegister(i);

				 if( (current_register != nullptr) )
				 {
					 this->packetsToSend[packetCount] = gcnew SerialPacket();

					 this->packetsToSend[packetCount]->Address = i;
					 this->packetsToSend[packetCount]->IsBatch = false;
					 this->packetsToSend[packetCount]->HasData = false;
					 this->packetsToSend[packetCount]->BatchLength = 0;
					 this->packetsToSend[packetCount]->ComputeChecksum();

					 packetCount++;
				 }
			 }

			 this->readingConfigData = true;
			 this->registersRead = 0;

			 this->configProgress = gcnew ProgressView();

			 this->configProgress->SetMaximum( packetCount );
			 this->configProgress->SetProgress( 0 );
			 this->configProgress->SetMinimum( 0 );

			 this->configProgress->Location = this->readButton->Location;
			 this->configProgress->Show();

			 serialConnector->TransmitPacket( packetsToSend[0] );
			 this->packetResponseTimer->Start();
		 }

private: System::Void label1_Click(System::Object^  sender, System::EventArgs^  e) {
		 }
private: System::Void getFirmwareVersionButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 SerialPacket^ fwRequest = gcnew SerialPacket();
			 fwRequest->HasData = false;
			 fwRequest->IsBatch = false;
			 fwRequest->Address = UM6_GET_FW_VERSION;

			 fwRequest->ComputeChecksum();

			 addPacket( fwRequest );
			 startTransmitting();
		 }

private: System::Void dataGraphButton_Click(System::Object^  sender, System::EventArgs^  e) {

			 if( currentGraphCount >= MAXIMUM_DATA_GRAPHS )
				 return;

			 if( SelectedFirmwareIndex == -1 )
				 return;

			 dataGraphs[currentGraphCount] = gcnew DataGraphDialog();
			 dataGraphs[currentGraphCount]->Disposed += gcnew System::EventHandler(this,&Form1::dataGraphClosed);
			 dataGraphs[currentGraphCount]->SetFirwarmwareReference( FirmwareArray[SelectedFirmwareIndex] );
			 
			 dataGraphs[currentGraphCount]->Show();
			 
			 currentGraphCount++;

			 // Uncheck boxes now that new graph has been created
			 for( UInt32 i = 0; i < FirmwareArray[SelectedFirmwareIndex]->GetDataItemCount(); i++ )
			 {
				 FirmwareItem^ current_item = FirmwareArray[SelectedFirmwareIndex]->GetDataItem( i );
				 current_item->Checked = false;
			 }

			 if( currentGraphCount == MAXIMUM_DATA_GRAPHS )
			 {
				 this->dataGraphButton->Enabled = false;
			 }
		 }
private: System::Void dataGraphClosed(System::Object^  sender, System::EventArgs^  e)
		 {
			 // Iterate through the list of open graph dialog boxes and remove the one that was just closed.
			 for( UInt32 i = 0; i < currentGraphCount; i++ )
			 {
				 if( dataGraphs[i]->IsDisposed )
				 {
					 for( UInt32 j = i+1; j < currentGraphCount; j++ )
					 {
						dataGraphs[j-1] = dataGraphs[j];
					 }

					 currentGraphCount--;
				 }
			 }

			 this->dataGraphButton->Enabled = true;
		 }

		 /* *****************************************************************************
		 * Name: packetResponseTimer_Tick
		 * Description: 
			
			If this timer fires, then it means a packet was sent and the expected response
			was not received within a reasonable amount of time.  This code will either 
			send the packet again, or if it has already retried more than a few times,
			then it gives up and removes everything from the TX buffer.

		 ** ****************************************************************************/
private: System::Void packetResponseTimer_Tick(System::Object^  sender, System::EventArgs^  e) 
		 {
			 if( this->packetRetryCount >= 3 )
			 {
				 this->addStatusTextSafe(L"Error: Timeout while communicating with sensor.", Color::Red);
				 this->packetCount = 0;
				 this->packetResponseTimer->Stop();
				 this->packetRetryCount = 0;

				 this->readingConfigData = false;
				 if( this->readingConfigData )
				 {
					this->configProgress->Close();
					this->readingConfigData = false;
				}

			 }
			 else
			 {
				 this->packetRetryCount++;
				 serialConnector->TransmitPacket( this->packetsToSend[0] );
			 }
		 }

		 // Write all configuration changes
private: System::Void writeButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->packetCount = 0;
			 this->packetResponseTimer->Stop();
			 
			 // Check all configuration registers.  If data has been changed, write that data to the sensor
			 for( int i = 0; i < DATA_REGISTER_START_ADDRESS; i++ )
			 {
				FirmwareRegister^ current_register = FirmwareArray[SelectedFirmwareIndex]->GetRegister(i);
				
				if( current_register != nullptr )
				{
					if( current_register->UserModified )
					{
						SerialPacket^ new_packet = gcnew SerialPacket();

						new_packet->Address = i;
						new_packet->IsBatch = false;
						new_packet->HasData = true;
						new_packet->BatchLength = 0;

						new_packet->SetDataByte(0, (unsigned char)((current_register->Contents >> 24) & 0x0FF) );
						new_packet->SetDataByte(1, (unsigned char)((current_register->Contents >> 16) & 0x0FF) );
						new_packet->SetDataByte(2, (unsigned char)((current_register->Contents >> 8) & 0x0FF) );
						new_packet->SetDataByte(3, (unsigned char)(current_register->Contents & 0x0FF) );

						new_packet->ComputeChecksum();

						addPacket( new_packet );
					}
				}
			 }

			 startTransmitting();			 
		 }

private: System::Void startTransmitting()
		 {
			 // If transmission has already started, return
			 if( this->packetResponseTimer->Enabled )
			 {
				 return;
			 }

			 // If there are packets to transmit, send them
			 if( this->packetCount > 0 )
			 {
				 serialConnector->TransmitPacket( this->packetsToSend[0] );
				 this->packetResponseTimer->Start();
			 }
		 }

private: System::Void addPacket( SerialPacket^ packet )
		 {
			 this->packetsToSend[packetCount] = packet;
			 this->packetCount++;
		 }

private: System::Void sendCommand( UInt32 address )
		 {
			 SerialPacket^ new_packet = gcnew SerialPacket();
			 new_packet->Address = address;
			 new_packet->IsBatch = false;
			 new_packet->HasData = false;
			 new_packet->BatchLength = 0;

			 new_packet->ComputeChecksum();

			 serialConnector->TransmitPacket( new_packet );
		 }

		 // When the mouse is double-clicked from within the "commands" list box, send the selected command to the sensor
private: System::Void commandsListBox_MouseDoubleClick(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) 
		 {
			 if( commandsListBox->SelectedItem != nullptr )
			 {
				String^ selected_text = dynamic_cast<String^>(commandsListBox->SelectedItem);
				UInt32 address = this->FirmwareArray[SelectedFirmwareIndex]->GetCommandAddress( selected_text );

				if( address != 0 )
				{
					sendCommand( address );
				}

				commandsListBox->SetSelected( commandsListBox->SelectedIndex, false );
			 }
		 }
private: System::Void flashWriteButton_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 SerialPacket^ new_packet = gcnew SerialPacket();
			 new_packet->Address = 171;
			 new_packet->IsBatch = false;
			 new_packet->HasData = false;
			 new_packet->BatchLength = 0;

			 new_packet->ComputeChecksum();

			 addPacket( new_packet );
			 startTransmitting();
		 }
private: System::Void buttonStartDataCollection_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->magDataCollectionEnabled = true;
			 this->buttonResetDataCollection->Enabled = true;
			 this->buttonStopDataCollection->Enabled = true;
			 this->buttonStartDataCollection->Enabled = false;

			 updateMagStatusLabelSafe( L"Collecting Data" );
		 }
private: System::Void buttonStopDataCollection_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 this->magDataCollectionEnabled = false;
			 
			 this->buttonStopDataCollection->Enabled = false;
			 this->buttonStartDataCollection->Enabled = true;

			 updateMagStatusLabelSafe( L"Stopped" );
		 }
private: System::Void buttonResetDataCollection_Click(System::Object^  sender, System::EventArgs^  e) {
			 this->magDataCollectionEnabled = false;

			 this->buttonResetDataCollection->Enabled = false;
			 this->buttonStartDataCollection->Enabled = true;
			 this->buttonStopDataCollection->Enabled = false;
			 this->buttonComputeCalibration->Enabled = false;
			 this->buttonWriteMagConfigToRAM->Enabled = false;

			 magMatrix00->Text = L"";
             magMatrix01->Text = L"";
             magMatrix02->Text = L"";

             magMatrix10->Text = L"";
             magMatrix11->Text = L"";
             magMatrix12->Text = L"";

             magMatrix20->Text = L"";
             magMatrix21->Text = L"";
             magMatrix22->Text = L"";

             magBiasX->Text = L"";
             magBiasY->Text = L"";
             magBiasZ->Text = L"";

			 this->rawMagDataPointer = 0;

			 updateMagCounterLabelSafe();
			 updateMagStatusLabelSafe( L"Idle" );
		 }
		 // Magnetometer calibration code.
private: System::Void buttonComputeCalibration_Click(System::Object^  sender, System::EventArgs^  e) 
		 {
			 GeneralMatrix^ D;
			 UInt32 i,j;

			 this->magDataCollectionEnabled = false;

			 this->buttonStartDataCollection->Enabled = false;
			 this->buttonStopDataCollection->Enabled = false;
			 this->buttonResetDataCollection->Enabled = false;

			 updateMagStatusLabelSafe( L"Computing Calibration" );

			 D = gcnew GeneralMatrix(this->rawMagDataPointer,10);
			 GeneralMatrix^ V;
			 SingularValueDecomposition^ SVD;
			 QRDecomposition^ QR;
			 CholeskyDecomposition^ Chol;
			 GeneralMatrix^ Ut;
			 GeneralMatrix^ U;
			 GeneralMatrix^ b;
			 GeneralMatrix^ v;
			 GeneralMatrix^ c;

			 double s;
			 double vnorm_sqrd;
			 
			 // Construct D matrix
             // D = [x.^2, y.^2, z.^2, x.*y, x.*z, y.*z, x, y, z, ones(N,1)];
			 for (i = 0; i < this->rawMagDataPointer; i++ )
			 {
                // x^2 term
				D->SetElement(i,0, this->rawMagData[i,0]*this->rawMagData[i,0]);

                // y^2 term
                D->SetElement(i,1, this->rawMagData[i,1]*this->rawMagData[i,1]);

                // z^2 term
                D->SetElement(i, 2, this->rawMagData[i, 2] * this->rawMagData[i, 2]);

                // x*y term
                D->SetElement(i,3,this->rawMagData[i,0]*this->rawMagData[i,1]);

                // x*z term
                D->SetElement(i,4,this->rawMagData[i,0]*this->rawMagData[i,2]);

                // y*z term
                D->SetElement(i,5,this->rawMagData[i,1]*this->rawMagData[i,2]);

                // x term
                D->SetElement(i,6,this->rawMagData[i,0]);

                // y term
                D->SetElement(i,7,this->rawMagData[i,1]);

                // z term
                D->SetElement(i,8,this->rawMagData[i,2]);

                // Constant term
                D->SetElement(i,9,1);
			 }

			 

            // QR=triu(qr(D))
			 try
			 {
				QR = gcnew QRDecomposition(D);
				// [U,S,V] = svd(D)
				SVD = gcnew SingularValueDecomposition(QR->R);
				V = SVD->GetV();
			 }
			 catch( Exception^ /*e*/ )
			 {
				updateMagStatusLabelSafe( L"Calibration Failed.  Please collect new (or more) raw data and try again." );

				this->buttonStartDataCollection->Enabled = true;
				this->buttonStopDataCollection->Enabled = false;
				this->buttonResetDataCollection->Enabled = true;

				return;
			 }

            GeneralMatrix^ A = gcnew GeneralMatrix(3, 3);

			cli::array<double,1>^ p = gcnew cli::array<double,1>(V->RowDimension);
            
            for (i = 0; i < (UInt32)V->RowDimension; i++ )
            {
                p[i] = V->GetElement(i,V->ColumnDimension-1);
            }
            
//          A = [p(1) p(4)/2 p(5)/2;
//          p(4)/2 p(2) p(6)/2; 
//          p(5)/2 p(6)/2 p(3)];

            if (p[0] < 0)
            {
                for (i = 0; i < (UInt32)V->RowDimension; i++)
                {
                    p[i] = -p[i];
                }
            }

            A->SetElement(0,0,p[0]);
            A->SetElement(0,1,p[3]/2);
            A->SetElement(0,2,p[4]/2);

            A->SetElement(1,0,p[3]/2);
            A->SetElement(1,1,p[1]);
            A->SetElement(1,2,p[5]/2);

            A->SetElement(2,0,p[4]/2);
            A->SetElement(2,1,p[5]/2);
            A->SetElement(2,2,p[2]);

			try {
				Chol = gcnew CholeskyDecomposition(A);
				Ut = Chol->GetL();
				U = Ut->Transpose();
			}
			catch( Exception^ /*e*/ )
			{
				updateMagStatusLabelSafe( L"Calibration Failed.  Please collect new (or more) raw data and try again." );

				this->buttonStartDataCollection->Enabled = true;
				this->buttonStopDataCollection->Enabled = false;
				this->buttonResetDataCollection->Enabled = true;

				return;
			}

			cli::array<double,1>^ bvect = gcnew cli::array<double,1>(3);
			bvect[0] = p[6]/2;
			bvect[1] = p[7]/2;
			bvect[2] = p[8]/2;
            double d = p[9];
            
			try {

				b = gcnew GeneralMatrix(bvect,3);
	            
				v = Ut->Solve(b);
	             
				vnorm_sqrd = v->GetElement(0,0)*v->GetElement(0,0) + v->GetElement(1,0)*v->GetElement(1,0) + v->GetElement(2,0)*v->GetElement(2,0);
				s = 1/Math::Sqrt(vnorm_sqrd - d);			

				c = U->Solve(v);

				for (i = 0; i < 3; i++)
				{
					c->SetElement(i, 0, -c->GetElement(i, 0));
				}
			}
			catch( Exception^ /*e*/ )
			{
				updateMagStatusLabelSafe( L"Calibration Failed.  Please collect new (or more) raw data and try again." );

				this->buttonStartDataCollection->Enabled = true;
				this->buttonStopDataCollection->Enabled = false;
				this->buttonResetDataCollection->Enabled = true;

				return;
			}

            U = U->Multiply(s);

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    calMat[i, j] = (float)U->GetElement(i, j);
                }
            }

            for (i = 0; i < 3; i++)
            {
				bias[i] = (Int16)Math::Round(c->GetElement(i, 0));
            }

			
			magMatrix00->Text = Convert::ToString(calMat[0, 0]);
            magMatrix01->Text = Convert::ToString(calMat[0, 1]);
            magMatrix02->Text = Convert::ToString(calMat[0, 2]);

            magMatrix10->Text = Convert::ToString(calMat[1, 0]);
            magMatrix11->Text = Convert::ToString(calMat[1, 1]);
            magMatrix12->Text = Convert::ToString(calMat[1, 2]);

            magMatrix20->Text = Convert::ToString(calMat[2, 0]);
            magMatrix21->Text = Convert::ToString(calMat[2, 1]);
            magMatrix22->Text = Convert::ToString(calMat[2, 2]);

            magBiasX->Text = bias[0]->ToString();
            magBiasY->Text = bias[1]->ToString();
            magBiasZ->Text = bias[2]->ToString();

            updateMagStatusLabelSafe("Done");

			this->buttonStartDataCollection->Enabled = true;
			this->buttonStopDataCollection->Enabled = false;
			this->buttonResetDataCollection->Enabled = true;
			this->buttonWriteMagConfigToRAM->Enabled = true;
		 }
private: System::Void buttonWriteMagConfigToRAM_Click(System::Object^  sender, System::EventArgs^  e) {

			 fConvert ftoi;

			 // Update registers with data and trigger a RAM Commit command
			 // Bias terms
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 15, (UInt32)((*this->bias[0] << 16) | (UInt32)(*this->bias[1] & 0x0FFFF)) );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 15, true );
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 16, (UInt32)(*this->bias[2] << 16) );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 16, true );

			 // Matrix entries
			 ftoi.float_val = this->calMat[0,0];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 35, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 35, true );

			 ftoi.float_val = this->calMat[0,1];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 36, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 36, true );

			 ftoi.float_val = this->calMat[0,2];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 37, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 37, true );

			 ftoi.float_val = this->calMat[1,0];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 38, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 38, true );

			 ftoi.float_val = this->calMat[1,1];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 39, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 39, true );

			 ftoi.float_val = this->calMat[1,2];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 40, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 40, true );

			 ftoi.float_val = this->calMat[2,0];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 41, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 41, true );

			 ftoi.float_val = this->calMat[2,1];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 42, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 42, true );

			 ftoi.float_val = this->calMat[2,2];
			 this->FirmwareArray[SelectedFirmwareIndex]->SetRegisterContents( 43, ftoi.uint32_val );
			 this->FirmwareArray[SelectedFirmwareIndex]->MarkRegisterAsUserModified( 43, true );


			 this->FirmwareArray[SelectedFirmwareIndex]->UpdateItemsFromRegisters( CONFIG_UPDATE );
			 
			 writeButton_Click( sender, e );			 
		 }
private: System::Void firmwareFileBrowseButton_Click(System::Object^  sender, System::EventArgs^  e) {
			 int returnval;
			 
			 this->openFileDialog1->ShowDialog();
			 
			 if( this->openFileDialog1->FileName != "" )
			 {
				 this->firmwareFileTextBox->Text = this->openFileDialog1->FileName;

				 // Attempt to open and parse the selected firmware file
				 returnval = firmwareProgrammer->LoadFirmware( this->firmwareFileTextBox->Text );

				 if( returnval == STM32_SUCCESS )
				 {
					 this->writeFirmwareButton->Enabled = true;

					 this->firmwareSizeLabel->Text = L"Firmware Size: " + this->firmwareProgrammer->FirmwareSizeKB + L" KB";
				 }
				 else
				 {
					 this->firmwareFileTextBox->Text = "";
					 
					 switch( returnval )
					 {
					 case STM32_ERROR_INVALID_HEXFILE:
						 this->addStatusTextSafe(L"ERROR - The specified hexfile was invalid.", Color::Red);
						 
						 break;

					 case STM32_ERROR_HEXFILE_OPEN_FAILED:
						 this->addStatusTextSafe(L"ERROR - Unable to open specified hexfile.", Color::Red);
						 
						 break;

					 default:
						 this->addStatusTextSafe(L"ERROR - Failed to parse hex file.", Color::Red);

						 break;
					 }
				 }
			 }
		 }
private: System::Void writeFirmwareButton_Click(System::Object^  sender, System::EventArgs^  e) {
			 int returnval;
			 
			 // If the serial port is already connected, disconnect.  We'll need to reconfigure the port to work with
			 // the firmware programming code.
			 if( this->serialConnector->IsOpen )
			 {
				 this->SerialDisconnect();
			 }

			 this->addStatusTextSafe(L"Connecting to Serial Port...", Color::Green);
			 returnval = firmwareProgrammer->OpenPort( dynamic_cast<String^>(this->serialComboBox->SelectedItem), int::Parse(dynamic_cast<String^>(this->serialBaudComboBox->SelectedItem)) );

			if( returnval != STM32_SUCCESS )
			{
				this->addStatusTextSafe(L"Failed to open serial port.  Is the port in use by another application?", Color::Red);
				return;
			}

			this->addStatusTextSafe(L"Programming device...", Color::Green);

			returnval = firmwareProgrammer->ProgramDevice( );

			 if( returnval == STM32_SUCCESS )
			 {
				 this->addStatusTextSafe(L"SUCCESS", Color::Green);
			 }
			 else
			 {
				 switch( returnval )
				 {
				 case STM32_ERROR_SERIAL_CONNECTION_FAILED:
					this->addStatusTextSafe(L"PROGRAMMING FAILED - Unable to access serial port.", Color::Red);

					 break;

				 case STM32_ERROR_DEVICE_UNAVAILABLE:
					 this->addStatusTextSafe(L"PROGRAMMING FAILED - Device unavailable.  Is the bootloader activated?", Color::Red);

					 break;

				 case STM32_ERROR_NACK_RECEIVED:
					 this->addStatusTextSafe(L"PROGRAMMING FAILED - Unable to write to device.  Make sure the device is not write protected and try again.", Color::Red);

					 break;

				 case STM32_ERROR_COMMUNICATION_LOST:
					this->addStatusTextSafe(L"PROGRAMMING FAILED - Lost connection to device while programming.  Please  try again.", Color::Red);

					break;

				 case STM32_ERROR_READBACK_MISMATCH:
					 this->addStatusTextSafe(L"PROGRAMMING FAILED - FLASH contents did not match after programming.  Please  try again.", Color::Red);

					 break;
				 }
			 }
			 firmwareProgrammer->Close();
		 }

private: void Form1::firmwareProgressUpdate_eventHandler( int progress )
		 {
			 this->firmwareProgressBar->Value = progress;
		 }
private: void Form1::firmwareStatusUpdate_eventHandler( String^ status_text )
		 {
			 this->firmwareStatusLabel->Text = status_text;
		 }
};
}

