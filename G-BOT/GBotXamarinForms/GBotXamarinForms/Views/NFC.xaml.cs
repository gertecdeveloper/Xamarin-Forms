using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.NFC;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//Baseado no exemplo do Plugin.NFC https://github.com/franckbour/Plugin.NFC

namespace GBotXamarinForms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NFC : ContentPage
    {
		public const string MIME_TYPE = "UTF-8";


		NFCNdefTypeFormat _type;

        bool _makeReadOnly = false;
        bool _eventsAlreadySubscribed = false;

		private bool _nfcIsEnabled;

		public bool NfcIsDisabled => !NfcIsEnabled;
		public bool NfcIsEnabled
		{
			get => _nfcIsEnabled;
			set
			{
				_nfcIsEnabled = value;
				OnPropertyChanged(nameof(NfcIsEnabled));
				OnPropertyChanged(nameof(NfcIsDisabled));
			}
		}


		private string labelTitulo;
		public string LabelTitulo
		{
			get { return labelTitulo; }
			set
			{
				labelTitulo = value;
				OnPropertyChanged(nameof(LabelTitulo)); // Notifica quando houver mudança
			}
		}

		private string labelResposta;
		public string LabelResposta
		{
			get { return labelResposta; }
			set
			{
				labelResposta = value;
				OnPropertyChanged(nameof(LabelResposta)); // Notifica quando houver mudança
			}
		}



		/// <summary>
		/// Propriedade que rastreia se o dispositivo Android ainda está ouvindo,
		/// para que possa indicar isso ao usuário.
		/// </summary>
		public bool DeviceIsListening
		{
			get => _deviceIsListening;
			set
			{
				_deviceIsListening = value;
				OnPropertyChanged(nameof(DeviceIsListening));
			}
		}
		private bool _deviceIsListening;

		public NFC()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
			BindingContext = this;
			LabelResposta = "";
			LabelTitulo = "NFC";
		}

		protected override bool OnBackButtonPressed()
		{
			UnsubscribeEvents();
			CrossNFC.Current.StopListening();
			return base.OnBackButtonPressed();
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();

			if (CrossNFC.IsSupported)
			{
				if (!CrossNFC.Current.IsAvailable)
					await ShowAlert("NFC não disponível neste device");

				NfcIsEnabled = CrossNFC.Current.IsEnabled;
				if (!NfcIsEnabled)
					await ShowAlert("NFC está desabilitado");


				// Custom NFC configuration (ex. UI messages in Portugues)
				CrossNFC.Current.SetConfiguration(new NfcConfiguration
				{
					Messages = new UserDefinedMessages
					{
						NFCWritingNotSupported = "A gravação de TAGs NFC não é compatível com este dispositivo",
						NFCDialogAlertMessage = "Aproxime seu dispositivo da etiqueta NFC",
						NFCErrorRead = "Erro de leitura. Tente novamente",
						NFCErrorEmptyTag = "Esta tag está vazia",
						NFCErrorReadOnlyTag = "Esta tag não é gravável",
						NFCErrorCapacityTag = "A capacidade deste TAG é muito baixa",
						NFCErrorMissingTag = "Nenhuma tag encontrada",
						NFCErrorMissingTagInfo = "Nenhuma informação para escrever na tag",
						NFCErrorNotSupportedTag = "Esta tag não é compatível",
						NFCErrorNotCompliantTag = "Esta tag não é compatível com NDEF",
						NFCErrorWrite = "Nenhuma informação para escrever na tag",
						NFCSuccessRead = "Leitura bem sucedida",
						NFCSuccessWrite = "Escrita com sucesso",
						NFCSuccessClear = "Formatação bem sucedida"
					}
				});

				SubscribeEvents();

				await BeginListening();
			}

		}

		/// <summary>
		/// Subscribe to the NFC events
		/// </summary>
		void SubscribeEvents()
		{
			if (_eventsAlreadySubscribed)
				return;

			_eventsAlreadySubscribed = true;

			CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
			CrossNFC.Current.OnMessagePublished += Current_OnMessagePublished;
			CrossNFC.Current.OnTagDiscovered += Current_OnTagDiscovered;
			CrossNFC.Current.OnNfcStatusChanged += Current_OnNfcStatusChanged;
			CrossNFC.Current.OnTagListeningStatusChanged += Current_OnTagListeningStatusChanged;

		}

		/// <summary>
		/// Unsubscribe from the NFC events
		/// </summary>
		void UnsubscribeEvents()
		{
			CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
			CrossNFC.Current.OnMessagePublished -= Current_OnMessagePublished;
			CrossNFC.Current.OnTagDiscovered -= Current_OnTagDiscovered;
			CrossNFC.Current.OnNfcStatusChanged -= Current_OnNfcStatusChanged;
			CrossNFC.Current.OnTagListeningStatusChanged += Current_OnTagListeningStatusChanged;
		}

		/// <summary>
		/// Event raised when Listener Status has changed
		/// </summary>
		/// <param name="isListening"></param>
		void Current_OnTagListeningStatusChanged(bool isListening) => DeviceIsListening = isListening;

		/// <summary>
		/// Event raised when NFC Status has changed
		/// </summary>
		/// <param name="isEnabled">NFC status</param>
		async void Current_OnNfcStatusChanged(bool isEnabled)
		{
			NfcIsEnabled = isEnabled;
			await ShowAlert($"NFC foi {(isEnabled ? "ativado" : "desativado")}");
		}

		/// <summary>
		/// Event raised when a NDEF message is received
		/// </summary>
		/// <param name="tagInfo">Received <see cref="ITagInfo"/></param>
		async void Current_OnMessageReceived(ITagInfo tagInfo)
		{
			if (tagInfo == null)
			{
				await ShowAlert("Tag não encontrada");
				return;
			}

			// Customized serial number
			var identifier = tagInfo.Identifier; 
			var serialNumber = NFCUtils.ByteArrayToHexString(identifier, ":");
			var title = "Tag Info";

			if (!tagInfo.IsSupported)
			{
				await ShowAlert("Esta não é uma tag compatível com NDEF!", title);
				LabelResposta = $"Id Cartão: {BitConverter.ToInt32(identifier, 0)}";
			}
			else if (tagInfo.IsEmpty)
			{
				await ShowAlert("Tag vazia", title);
			}
			else
			{
				var first = tagInfo.Records[0];
				LabelResposta = $"Id Cartão: {BitConverter.ToInt32(identifier, 0)} \n{GetMessage(first)}";
			}
		}
				

		/// <summary>
		/// Event raised when data has been published on the tag
		/// </summary>
		/// <param name="tagInfo">Published <see cref="ITagInfo"/></param>
		async void Current_OnMessagePublished(ITagInfo tagInfo)
		{
			try
			{
				
				CrossNFC.Current.StopPublishing();
				if (tagInfo.IsEmpty)
					await ShowAlert("A Formatação da Tag foi bem-sucedida");
				else
					await ShowAlert("Escrita da Tag com sucesso");
			}
			catch (Exception ex)
			{
				await ShowAlert(ex.Message);
			}
		}

		/// <summary>
		/// Event raised when a NFC Tag is discovered
		/// </summary>
		/// <param name="tagInfo"><see cref="ITagInfo"/> to be published</param>
		/// <param name="format">Format the tag</param>
		async void Current_OnTagDiscovered(ITagInfo tagInfo, bool format)
		{
			if (!CrossNFC.Current.IsWritingTagSupported)
			{
				await ShowAlert("A Gravação da Tag não é compatível com este dispositivo");
				return;
			}            

			try
			{
				NFCNdefRecord record = null;
				switch (_type)
				{
					case NFCNdefTypeFormat.WellKnown:
						record = new NFCNdefRecord
						{
							TypeFormat = NFCNdefTypeFormat.WellKnown,
							MimeType = MIME_TYPE,
							Payload = NFCUtils.EncodeToByteArray(entry_mensagem.Text)
						};
						break;
					case NFCNdefTypeFormat.Uri:
						record = new NFCNdefRecord
						{
							TypeFormat = NFCNdefTypeFormat.Uri,
							Payload = NFCUtils.EncodeToByteArray("https://github.com/franckbour/Plugin.NFC")
						};
						break;
					case NFCNdefTypeFormat.Mime:
						record = new NFCNdefRecord
						{
							TypeFormat = NFCNdefTypeFormat.Mime,
							MimeType = MIME_TYPE,
							Payload = NFCUtils.EncodeToByteArray("Plugin.NFC is awesome!")
						};
						break;
					default:
						break;
				}

				if (!format && record == null)
					throw new Exception("Record can't be null.");

				tagInfo.Records = new[] { record };

				if (format)
					CrossNFC.Current.ClearMessage(tagInfo);
				else
				{
					CrossNFC.Current.PublishMessage(tagInfo, _makeReadOnly);
				}
			}
			catch (Exception ex)
			{
				await ShowAlert(ex.Message);
			}
		}

		/// <summary>
		/// Returns the tag information from NDEF record
		/// </summary>
		/// <param name="record"><see cref="NFCNdefRecord"/></param>
		/// <returns>The tag information</returns>
		string GetMessage(NFCNdefRecord record)
		{
			var message = $"Mensagem: {record.Message}";

			return message;
		}

		/// <summary>
		/// Display an alert
		/// </summary>
		/// <param name="message">Message to be displayed</param>
		/// <param name="title">Alert title</param>
		/// <returns>The task to be performed</returns>
		Task ShowAlert(string message, string title = null) => DisplayAlert(string.IsNullOrWhiteSpace(title) ? "NFC" : title, message, "OK");

		/// <summary>
		/// Task to safely start listening for NFC Tags
		/// </summary>
		/// <returns>The task to be performed</returns>
		async Task BeginListening()
		{
			try
			{
				CrossNFC.Current.StartListening();
			}
			catch (Exception ex)
			{
				await ShowAlert(ex.Message);
			}
		}

		/// <summary>
		/// Task to safely stop listening for NFC tags
		/// </summary>
		/// <returns>The task to be performed</returns>
		async Task StopListening()
		{
			try
			{
				CrossNFC.Current.StopListening();
			}
			catch (Exception ex)
			{
				await ShowAlert(ex.Message);
			}
		}

		private async void Button_Clicked_Gravar(object sender, EventArgs e)
		{
			LabelTitulo = "Gravar Cartão";
			LabelResposta = "Aproxime o Cartão";

			if (String.IsNullOrWhiteSpace(entry_mensagem.Text))
			{
				await ShowAlert("Digite uma mensagem para ser gravada no Cartão!", "Atenção");
				return;
			}

			await Publish(NFCNdefTypeFormat.WellKnown);
		}

		private async void Button_Clicked_Ler(object sender, EventArgs e)
		{
			LabelTitulo = "Leitura do Cartão NFC";
			LabelResposta = "Aproxime o Cartão";
			await BeginListening();
		}

		private async void Button_Clicked_Formatar(object sender, EventArgs e)
		{
			LabelTitulo = "Formatar Cartão";
			LabelResposta = "Aproxime o Cartão";
			await Publish();
		}

		private async void Button_Clicked_Gravar_Ler(object sender, EventArgs e) => await ShowAlert("Operação não suportada pelo Plugin");

		/// <summary>
		/// Task to publish data to the tag
		/// </summary>
		/// <param name="type"><see cref="NFCNdefTypeFormat"/></param>
		/// <returns>The task to be performed</returns>
		async Task Publish(NFCNdefTypeFormat? type = null)
		{
			await BeginListening();
			try
			{
				// Perigo pode tornar a TAG readonly pra sempre
				_makeReadOnly = false;

				if (type.HasValue) _type = type.Value;
				CrossNFC.Current.StartPublishing(!type.HasValue);
			}
			catch (Exception ex)
			{
				await ShowAlert(ex.Message);
			}
		}
	}

}
