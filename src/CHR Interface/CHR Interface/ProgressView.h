#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;


namespace CHRInterface {

	/// <summary>
	/// Summary for ProgressView
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class ProgressView : public System::Windows::Forms::Form
	{
	public:
		ProgressView(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

		void SetProgress( int progress )
		{
			progressBar->Value = progress;
			if( progress == progressBar->Maximum )
			{
				timer1->Start();
			}
		}

		void SetMaximum( int maximum )
		{
			progressBar->Maximum = maximum;
		}

		void SetMinimum( int minimum )
		{
			progressBar->Minimum = minimum;
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~ProgressView()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::ProgressBar^  progressBar;
	private: System::Windows::Forms::Timer^  timer1;
	private: System::ComponentModel::IContainer^  components;
	protected: 

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
			this->progressBar = (gcnew System::Windows::Forms::ProgressBar());
			this->timer1 = (gcnew System::Windows::Forms::Timer(this->components));
			this->SuspendLayout();
			// 
			// progressBar
			// 
			this->progressBar->Location = System::Drawing::Point(13, 12);
			this->progressBar->MarqueeAnimationSpeed = 1;
			this->progressBar->Maximum = 50;
			this->progressBar->Name = L"progressBar";
			this->progressBar->Size = System::Drawing::Size(303, 23);
			this->progressBar->Step = 1;
			this->progressBar->Style = System::Windows::Forms::ProgressBarStyle::Continuous;
			this->progressBar->TabIndex = 0;
			// 
			// timer1
			// 
			this->timer1->Interval = 600;
			this->timer1->Tick += gcnew System::EventHandler(this, &ProgressView::timer1_Tick);
			// 
			// ProgressView
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(328, 49);
			this->Controls->Add(this->progressBar);
			this->Name = L"ProgressView";
			this->Text = L"Reading Configuration Settings";
			this->TopMost = true;
			this->Shown += gcnew System::EventHandler(this, &ProgressView::ProgressView_Shown);
			this->VisibleChanged += gcnew System::EventHandler(this, &ProgressView::ProgressView_VisibleChanged);
			this->ResumeLayout(false);

		}
#pragma endregion
	private: System::Void timer1_Tick(System::Object^  sender, System::EventArgs^  e) 
			 {
				 this->Close();
			 }
	private: System::Void ProgressView_Shown(System::Object^  sender, System::EventArgs^  e) {
				 
			 }
	private: System::Void ProgressView_VisibleChanged(System::Object^  sender, System::EventArgs^  e) {
	
			 }
};
}
