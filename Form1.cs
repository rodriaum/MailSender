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

        private void Logger(string message, bool isError)
        {
            inputTextBox.Clear();
            inputTextBox.ForeColor = isError ? Color.Red : Color.Green;
            inputTextBox.Text = message;
        }

        private void addMailButon_Click(object sender, EventArgs e)
        {
            mailCheckedListBox.Items.Add(changeListTextBox.Text);
            // Coloca o novo mail adicionado como marcado, para facilitar.
            mailCheckedListBox.SetItemChecked(mailCheckedListBox.Items.Count - 1, true);
        }

        private void removeMailButton_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();

            // A lista � qual este enumerador est� vinculado foi modificada. Um enumerador s� pode ser usado se a lista n�o mudar.
            foreach (string s in mailCheckedListBox.Items)
                list.Add(s);

            foreach (string s in list)
                mailCheckedListBox.Items.Remove(s);
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
                Logger("N�o foi poss�vel estabelecer conex�o com o servidor!\n" + ex.Message, true);
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
                            bodyTextBox.Text
                        ));

                        Logger("Mail enviado para os destinat�rios.", false);
                    }
                }
                catch (Exception ex)
                {
                    Logger("N�o foi poss�vel enviar o mail!\n" + ex.Message, true);
                }
            }
        }
    }
}