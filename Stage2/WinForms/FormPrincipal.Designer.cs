namespace WinFormsApp
{
    partial class FormPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelDessin = new System.Windows.Forms.Panel();
            this.btnDessiner = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.panelDessin.BackColor = System.Drawing.Color.White;
            this.panelDessin.Location = new System.Drawing.Point(12, 12);
            this.panelDessin.Name = "panelDessin";
            this.panelDessin.Size = new System.Drawing.Size(760, 500);
            this.panelDessin.TabIndex = 0;
            this.panelDessin.Paint += new System.Windows.Forms.PaintEventHandler(this.panelDessin_Paint);
            this.btnDessiner.Location = new System.Drawing.Point(12, 518);
            this.btnDessiner.Name = "btnDessiner";
            this.btnDessiner.Size = new System.Drawing.Size(100, 30);
            this.btnDessiner.TabIndex = 1;
            this.btnDessiner.Text = "Dessiner";
            this.btnDessiner.UseVisualStyleBackColor = true;
            this.btnDessiner.Click += new System.EventHandler(this.btnDessiner_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.btnDessiner);
            this.Controls.Add(this.panelDessin);
            this.Name = "FormPrincipal";
            this.Text = "Dessin - WinForms";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelDessin;
        private System.Windows.Forms.Button btnDessiner;
    }
}
