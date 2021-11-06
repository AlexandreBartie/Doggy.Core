
namespace MeuSeleniumCSharp
{
    partial class frmHubTeste
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdExecutar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdExecutar
            // 
            this.cmdExecutar.Location = new System.Drawing.Point(344, 198);
            this.cmdExecutar.Name = "cmdExecutar";
            this.cmdExecutar.Size = new System.Drawing.Size(167, 49);
            this.cmdExecutar.TabIndex = 0;
            this.cmdExecutar.Text = "Executar";
            this.cmdExecutar.UseVisualStyleBackColor = true;
            this.cmdExecutar.Click += new System.EventHandler(this.cmdExecutar_Click);
            // 
            // frmHubTeste
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cmdExecutar);
            this.Name = "frmHubTeste";
            this.Text = "Hub de Teste";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdExecutar;
    }
}

