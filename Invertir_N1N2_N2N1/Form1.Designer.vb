<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmInvertirNudos
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.CmdAceptar = New System.Windows.Forms.Button()
        Me.FrameContenedor = New System.Windows.Forms.GroupBox()
        Me.CmdAbrir = New System.Windows.Forms.Button()
        Me.TxtINP = New System.Windows.Forms.TextBox()
        Me.LblINP = New System.Windows.Forms.Label()
        Me.OpenFD = New System.Windows.Forms.OpenFileDialog()
        Me.FrameContenedor.SuspendLayout()
        Me.SuspendLayout()
        '
        'CmdAceptar
        '
        Me.CmdAceptar.Location = New System.Drawing.Point(229, 96)
        Me.CmdAceptar.Name = "CmdAceptar"
        Me.CmdAceptar.Size = New System.Drawing.Size(64, 23)
        Me.CmdAceptar.TabIndex = 7
        Me.CmdAceptar.Text = "Aceptar"
        Me.CmdAceptar.UseVisualStyleBackColor = True
        '
        'FrameContenedor
        '
        Me.FrameContenedor.Controls.Add(Me.CmdAbrir)
        Me.FrameContenedor.Controls.Add(Me.TxtINP)
        Me.FrameContenedor.Controls.Add(Me.LblINP)
        Me.FrameContenedor.Location = New System.Drawing.Point(12, 12)
        Me.FrameContenedor.Name = "FrameContenedor"
        Me.FrameContenedor.Size = New System.Drawing.Size(282, 78)
        Me.FrameContenedor.TabIndex = 6
        Me.FrameContenedor.TabStop = False
        '
        'CmdAbrir
        '
        Me.CmdAbrir.Location = New System.Drawing.Point(217, 42)
        Me.CmdAbrir.Name = "CmdAbrir"
        Me.CmdAbrir.Size = New System.Drawing.Size(42, 20)
        Me.CmdAbrir.TabIndex = 2
        Me.CmdAbrir.Text = "..."
        Me.CmdAbrir.UseVisualStyleBackColor = True
        '
        'TxtINP
        '
        Me.TxtINP.Location = New System.Drawing.Point(18, 42)
        Me.TxtINP.Name = "TxtINP"
        Me.TxtINP.Size = New System.Drawing.Size(193, 20)
        Me.TxtINP.TabIndex = 1
        '
        'LblINP
        '
        Me.LblINP.AutoSize = True
        Me.LblINP.Location = New System.Drawing.Point(15, 26)
        Me.LblINP.Name = "LblINP"
        Me.LblINP.Size = New System.Drawing.Size(179, 13)
        Me.LblINP.TabIndex = 0
        Me.LblINP.Text = "Seleccione el fichero INP de Epanet"
        '
        'FrmInvertirNudos
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(302, 127)
        Me.Controls.Add(Me.CmdAceptar)
        Me.Controls.Add(Me.FrameContenedor)
        Me.Name = "FrmInvertirNudos"
        Me.Text = "Invertir N1N2_N2N1"
        Me.FrameContenedor.ResumeLayout(False)
        Me.FrameContenedor.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents CmdAceptar As Button
    Friend WithEvents FrameContenedor As GroupBox
    Friend WithEvents CmdAbrir As Button
    Friend WithEvents TxtINP As TextBox
    Friend WithEvents LblINP As Label
    Friend WithEvents OpenFD As OpenFileDialog
End Class
