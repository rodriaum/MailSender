using MailSender.client;
using System.Net.Mail;

/**
 * Author: Rodriaum (Rodrigo Ferreira)
 * Este arquivo est� licenciado sob a MIT License.
 * https://github.com/rodriaum/MailSender?tab=MIT-1-ov-file#readme
 */

namespace MailSender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string? FILE { get; set; }

        private async void Logger(string message, bool isError)
        {
            inputTextBox.Clear();
            inputTextBox.ForeColor = isError ? Color.Red : Color.Green;
            inputTextBox.Text = message;

            // Espera 3s/3000ms at� limpar o texto novamente. 
            await Task.Delay(3000);
            inputTextBox.Clear();
        }

        private void addMailButon_Click(object sender, EventArgs e)
        {
            mailCheckedListBox.Items.Add(changeListTextBox.Text);
            // Coloca o novo mail adicionado como marcado, para facilitar.
            mailCheckedListBox.SetItemChecked(mailCheckedListBox.Items.Count - 1, true);

            // Limpa Text Box onde � inserido o novo mail.
            changeListTextBox.Clear();
        }

        private void removeMailButton_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();

            // A lista � qual este enumerador est� vinculado foi modificada. Um enumerador s� pode ser usado se a lista n�o mudar.
            foreach (string s in mailCheckedListBox.Items)
                list.Add(s);

            foreach (string s in list)
                mailCheckedListBox.Items.Remove(s);

            // Limpa Text Box onde � inserido o novo mail.
            changeListTextBox.Clear();
        }

        private void sendMailButton_Click(object sender, EventArgs e)
        {
            SmtpClient? client = null;

            try
            {
                client = SmtpHelper.Connection(
                   hostAddressTextBox.Text,
                   Convert.ToInt16(hostPortTextBox.Text),
                   senderAddressTextBox.Text,
                   addressPasswordTextBox.Text
               );
            }
            catch (Exception ex)
            {
                Logger("N�o foi poss�vel estabelecer conex�o com o servidor: " + ex.Message, true);
            }
            finally
            {
                try
                {
                    foreach (string recipients in mailCheckedListBox.CheckedItems)
                    {
                        SmtpHelper.Send(client, SmtpHelper.Message(
                            senderAddressTextBox.Text,
                            recipients,
                            subjectTextBox.Text,
                            bodyTextBox.Text,
                            FILE
                        ));

                        Logger("Mail enviado para os destinat�rio(s).", false);
                    }
                }
                catch (Exception ex)
                {
                    Logger("N�o foi poss�vel enviar o mail: " + ex.Message, true);
                }

                // Ap�s finalizar tudo, o sistema elimina o arquivo da pasta do programa.
                try
                {
                    File.Delete(FILE);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void fileUploadButton_Click(object sender, EventArgs e)
        {
            // Abre a janela, para o usu�rio procurar o arquivo que pretende anexar.
            DialogResult dialog = openMailFileDialog.ShowDialog();

            // Pega o ficheiro .dll do programa atual, pois no futuro s� ser� utilizada a pasta onde o ficheiro .dll fica.
            System.Reflection.Assembly? assembly = System.Reflection.Assembly.GetEntryAssembly();

            if (assembly != null)
            {
                // Pega o diret�rio do programa e acrescenta o nome do ficheiro para onde vai ser copiado.
                string? nextLocation = Path.GetDirectoryName(assembly.Location) + "\\" + openMailFileDialog.SafeFileName;

                if (nextLocation != null)
                {
                    // Se o usu�rio anexar um arquivo, ir� retornar OK, e assim, ir� continuar.
                    if (dialog == DialogResult.OK)
                    {
                        FILE = openMailFileDialog.SafeFileName;
                        FileTextBox.Text = FILE;

                        // Tenta copiar o arquivo anexado pelo usu�rio para a pasta do programa, assim, facilita a anexa��o do arquivo no mail com base na class Attachment.
                        try
                        {
                            File.Copy(@openMailFileDialog.FileName, nextLocation);
                        }
                        catch (Exception ex)
                        {
                            Logger("N�o foi poss�vel importar o ficheiro: " + ex.Message, true);
                        }

                        // Mensagem de OK.
                        Logger("Ficheiro anexado com sucesso.", false);
                    }
                    else if (dialog == DialogResult.Abort)
                    {
                        Logger("N�o foi poss�vel anexar o ficheiro.", false);
                    }
                }
                else
                {
                    Logger("N�o foi poss�vel converter o diret�rio do programa.", true);
                }
            }
            else
            {
                Logger("N�o foi poss�vel pegar o diret�rio do programa.", true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileTextBox.Clear();
            FILE = null;
        }
    }
}