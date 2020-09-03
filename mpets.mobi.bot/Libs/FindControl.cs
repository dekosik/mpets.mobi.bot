using System.Linq;
using System.Windows.Forms;

namespace mpets.mobi.bot.Libs
{
    internal class FindControl
    {
        public TextBox FindTextBox(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as TextBox;
            }

            return new TextBox();
        }

        public Label FindLabel(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as Label;
            }

            return new Label();
        }

        public NumericUpDown FindNumericUpDown(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as NumericUpDown;
            }

            return new NumericUpDown();
        }

        public ToolStrip FindToolStrip(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as ToolStrip;
            }

            return new ToolStrip();
        }

        public Button FindButton(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as Button;
            }

            return new Button();
        }

        public RichTextBox FindRichTextBox(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as RichTextBox;
            }

            return new RichTextBox();
        }

        public TabPage FindTabPage(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as TabPage;
            }

            return new TabPage();
        }

        public CheckBox FindCheckBox(string name, int id, Form1 form)
        {
            Control[] controls = form.Controls.Find(name + id.ToString(), true);

            if (controls.Any())
            {
                return controls.First() as CheckBox;
            }

            return new CheckBox();
        }
    }
}
