﻿Imports iBMSC.Editor

Partial Public Class MainWindow

    Private Sub PMainInPreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles PMainIn.PreviewKeyDown, PMainInL.PreviewKeyDown, PMainInR.PreviewKeyDown
        If e.KeyCode = Keys.ShiftKey Or e.KeyCode = Keys.ControlKey Then
            RefreshPanelAll()
            POStatusRefresh()
            Exit Sub
        End If

        If e.KeyCode = 18 Then Exit Sub

        Dim iI As Integer = sender.Tag
        Dim xI1 As Integer
        Dim xTargetColumn As Integer = -1
        Dim xUndo As UndoRedo.LinkedURCmd = Nothing
        Dim xRedo As UndoRedo.LinkedURCmd = New UndoRedo.Void
        Dim xBaseRedo As UndoRedo.LinkedURCmd = xRedo
        ReDim uNotes(-1)

        Select Case e.KeyCode
            Case Keys.Up
                Dim xVPosition As Double = 192 / gDivide
                If My.Computer.Keyboard.CtrlKeyDown Then xVPosition = 1

                'Ks cannot be beyond the upper boundary
                Dim muVPosition As Double = VPosition1000() - 1
                For xI1 = 1 To UBound(Notes)
                    If Notes(xI1).Selected Then
                        'K(xI1).VPosition = Math.Floor(K(xI1).VPosition / (192 / gDivide)) * 192 / gDivide
                        muVPosition = IIf(Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0) + xVPosition > muVPosition,
                                                          Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0) + xVPosition,
                                                          muVPosition)
                    End If
                Next
                muVPosition -= 191999

                'xRedo = sCmdKMs(0, xVPosition - muVPosition, True)
                Dim xVPos As Double
                For xI1 = 1 To UBound(Notes)
                    If Not Notes(xI1).Selected Then Continue For

                    xVPos = Notes(xI1).VPosition + xVPosition - muVPosition
                    Me.RedoMoveNote(Notes(xI1), Notes(xI1).ColumnIndex, xVPos, True, xUndo, xRedo)
                    Notes(xI1).VPosition = xVPos
                Next
                'xUndo = sCmdKMs(0, -xVPosition + muVPosition, True)

                If xVPosition - muVPosition <> 0 Then AddUndo(xUndo, xBaseRedo.Next)
                SortByVPositionInsertion()
                UpdatePairing()
                CalculateTotalNotes()
                CalculateGreatestVPosition()
                RefreshPanelAll()

            Case Keys.Down
                Dim xVPosition As Double = -192 / gDivide
                If My.Computer.Keyboard.CtrlKeyDown Then xVPosition = -1

                'Ks cannot be beyond the lower boundary
                Dim mVPosition As Double = 0
                For xI1 = 1 To UBound(Notes)
                    If Notes(xI1).Selected Then
                        'K(xI1).VPosition = Math.Ceiling(K(xI1).VPosition / (192 / gDivide)) * 192 / gDivide
                        mVPosition = IIf(Notes(xI1).VPosition + xVPosition < mVPosition,
                                                                 Notes(xI1).VPosition + xVPosition,
                                                                 mVPosition)
                    End If
                Next

                'xRedo = sCmdKMs(0, xVPosition - mVPosition, True)
                Dim xVPos As Double
                For xI1 = 1 To UBound(Notes)
                    If Not Notes(xI1).Selected Then Continue For

                    xVPos = Notes(xI1).VPosition + xVPosition - mVPosition
                    Me.RedoMoveNote(Notes(xI1), Notes(xI1).ColumnIndex, xVPos, True, xUndo, xRedo)
                    Notes(xI1).VPosition = xVPos
                Next
                'xUndo = sCmdKMs(0, -xVPosition + mVPosition, True)

                If xVPosition - mVPosition <> 0 Then AddUndo(xUndo, xBaseRedo.Next)
                SortByVPositionInsertion()
                UpdatePairing()
                CalculateTotalNotes()
                CalculateGreatestVPosition()
                RefreshPanelAll()

            Case Keys.Left
                'For xI1 = 1 To UBound(K)
                '    If K(xI1).Selected Then K(xI1).ColumnIndex = RealColumnToEnabled(K(xI1).ColumnIndex) - 1
                'Next

                'Ks cannot be beyond the left boundary
                Dim mLeft As Integer = 0
                For xI1 = 1 To UBound(Notes)
                    If Notes(xI1).Selected Then mLeft = IIf(RealColumnToEnabled(Notes(xI1).ColumnIndex) - 1 < mLeft,
                                                        RealColumnToEnabled(Notes(xI1).ColumnIndex) - 1,
                                                        mLeft)
                Next
                'xRedo = sCmdKMs(-1 - mLeft, 0, True)
                Dim xCol As Integer
                For xI1 = 1 To UBound(Notes)
                    If Not Notes(xI1).Selected Then Continue For

                    xCol = EnabledColumnToReal(RealColumnToEnabled(Notes(xI1).ColumnIndex) - 1 - mLeft)
                    Me.RedoMoveNote(Notes(xI1), xCol, Notes(xI1).VPosition, True, xUndo, xRedo)
                    Notes(xI1).ColumnIndex = xCol
                Next
                'xUndo = sCmdKMs(1 + mLeft, 0, True)

                If -1 - mLeft <> 0 Then AddUndo(xUndo, xBaseRedo.Next)
                UpdatePairing()
                CalculateTotalNotes()
                RefreshPanelAll()

            Case Keys.Right
                'xRedo = sCmdKMs(1, 0, True)
                Dim xCol As Integer
                For xI1 = 1 To UBound(Notes)
                    If Not Notes(xI1).Selected Then Continue For

                    xCol = EnabledColumnToReal(RealColumnToEnabled(Notes(xI1).ColumnIndex) + 1)
                    Me.RedoMoveNote(Notes(xI1), xCol, Notes(xI1).VPosition, True, xUndo, xRedo)
                    Notes(xI1).ColumnIndex = xCol
                Next
                'xUndo = sCmdKMs(-1, 0, True)

                AddUndo(xUndo, xBaseRedo.Next)
                UpdatePairing()
                CalculateTotalNotes()
                RefreshPanelAll()

            Case Keys.Delete
                mnDelete_Click(mnDelete, New System.EventArgs)

            Case Keys.Home
                If spFocus = 0 Then VSL.Value = 0
                If spFocus = 1 Then VS.Value = 0
                If spFocus = 2 Then VSR.Value = 0

            Case Keys.End
                If spFocus = 0 Then VSL.Value = VSL.Minimum
                If spFocus = 1 Then VS.Value = VS.Minimum
                If spFocus = 2 Then VSR.Value = VSR.Minimum

            Case Keys.PageUp
                If spFocus = 0 Then VSL.Value = IIf(VSL.Value - gPgUpDn > VSL.Minimum, VSL.Value - gPgUpDn, VSL.Minimum)
                If spFocus = 1 Then VS.Value = IIf(VS.Value - gPgUpDn > VS.Minimum, VS.Value - gPgUpDn, VS.Minimum)
                If spFocus = 2 Then VSR.Value = IIf(VSR.Value - gPgUpDn > VSR.Minimum, VSR.Value - gPgUpDn, VSR.Minimum)

            Case Keys.PageDown
                If spFocus = 0 Then VSL.Value = IIf(VSL.Value + gPgUpDn < 0, VSL.Value + gPgUpDn, 0)
                If spFocus = 1 Then VS.Value = IIf(VS.Value + gPgUpDn < 0, VS.Value + gPgUpDn, 0)
                If spFocus = 2 Then VSR.Value = IIf(VSR.Value + gPgUpDn < 0, VSR.Value + gPgUpDn, 0)

            Case Keys.Oemcomma
                If gDivide * 2 <= CGDivide.Maximum Then CGDivide.Value = gDivide * 2

            Case Keys.OemPeriod
                If gDivide \ 2 >= CGDivide.Minimum Then CGDivide.Value = gDivide \ 2

            Case Keys.OemQuestion
                'Dim xTempSwap As Integer = gSlash
                'gSlash = CGDivide.Value
                'CGDivide.Value = xTempSwap
                CGDivide.Value = gSlash

            Case Keys.Oemplus
                With CGHeight
                    .Value += IIf(.Value > .Maximum - .Increment, .Maximum - .Value, .Increment)
                End With

            Case Keys.OemMinus
                With CGHeight
                    .Value -= IIf(.Value < .Minimum + .Increment, .Value - .Minimum, .Increment)
                End With

            Case Keys.Add
                If LWAV.SelectedIndex = -1 Then
                    LWAV.SelectedIndex = 0
                Else
                    Dim newIndex As Integer = LWAV.SelectedIndex + 1
                    If newIndex > LWAV.Items.Count - 1 Then newIndex = LWAV.Items.Count - 1
                    LWAV.SelectedIndices.Clear()
                    LWAV.SelectedIndex = newIndex
                    validate_LWAV_view()
                End If

            Case Keys.Subtract
                If LWAV.SelectedIndex = -1 Then
                    LWAV.SelectedIndex = 0
                Else
                    Dim newIndex As Integer = LWAV.SelectedIndex - 1
                    If newIndex < 0 Then newIndex = 0
                    LWAV.SelectedIndices.Clear()
                    LWAV.SelectedIndex = newIndex
                End If

            Case Keys.G
                CGSnap.Checked = Not gSnap

            Case Keys.L
                If Not My.Computer.Keyboard.CtrlKeyDown Then POBLong_Click(Nothing, Nothing)

            Case Keys.S
                If Not My.Computer.Keyboard.CtrlKeyDown Then POBNormal_Click(Nothing, Nothing)

            Case Keys.D
                CGDisableVertical.Checked = Not CGDisableVertical.Checked

            Case Keys.NumPad0, Keys.D0
                For xI2 As Integer = 1 To UBound(Notes)
                    If Not Notes(xI2).Selected Then Continue For

                    With Notes(xI2)
                        Dim xxCol As Integer = niB

                        'TODO: optimize the for loops below
                        If NTInput Then
                            For xI0 As Integer = 1 To UBound(Notes)
                                If Notes(xI0).ColumnIndex = xxCol AndAlso
                                    Notes(xI0).VPosition <= Notes(xI2).VPosition + Notes(xI2).Length And Notes(xI0).VPosition + Notes(xI0).Length >= Notes(xI2).VPosition Then _
                                    xxCol += 1 : xI0 = 1
                                'If K(xI0).ColumnIndex = xxCol AndAlso _
                                'IIf(K(xI0).Length > 0, _
                                '    IIf(.Length = 0, _
                                '        K(xI0).VPosition <= K(xI2).VPosition And K(xI0).VPosition + K(xI0).Length >= K(xI2).VPosition, _
                                '        K(xI0).VPosition <= K(xI2).VPosition + K(xI2).Length And K(xI0).VPosition + K(xI0).Length >= K(xI2).VPosition), _
                                '    IIf(.Length = 0, _
                                '        K(xI0).VPosition = K(xI2).VPosition, _
                                '        K(xI2).VPosition <= K(xI0).VPosition And K(xI2).VPosition + K(xI2).Length >= K(xI0).VPosition)) Then _
                                'xxCol += 1 : xI0 = 1
                                'If K(xI0).VPosition > K(xI2).VPosition + IIf(K(xI2).Length > 0.0#, K(xI2).Length, 0.0#) Then Exit For
                            Next
                        Else
                            For xI0 As Integer = 1 To UBound(Notes)
                                If Notes(xI0).ColumnIndex = xxCol AndAlso Notes(xI0).VPosition = Notes(xI2).VPosition Then _
                                    xxCol += 1 : xI0 = 1
                                'If K(xI0).VPosition > K(xI2).VPosition Then Exit For
                            Next
                        End If

                        Me.RedoMoveNote(Notes(xI2), xxCol, .VPosition, True, xUndo, xRedo)
                        .ColumnIndex = xxCol
                    End With
                Next
                AddUndo(xUndo, xBaseRedo.Next)
                UpdatePairing()
                CalculateTotalNotes()
                RefreshPanelAll()

            Case Keys.Oem1, Keys.NumPad1, Keys.D1 : xTargetColumn = niA1 : GoTo MoveToColumn
            Case Keys.Oem2, Keys.NumPad2, Keys.D2 : xTargetColumn = niA2 : GoTo MoveToColumn
            Case Keys.Oem3, Keys.NumPad3, Keys.D3 : xTargetColumn = niA3 : GoTo MoveToColumn
            Case Keys.Oem4, Keys.NumPad4, Keys.D4 : xTargetColumn = niA4 : GoTo MoveToColumn
            Case Keys.Oem5, Keys.NumPad5, Keys.D5 : xTargetColumn = niA5 : GoTo MoveToColumn
            Case Keys.Oem6, Keys.NumPad6, Keys.D6 : xTargetColumn = niA6 : GoTo MoveToColumn
            Case Keys.Oem7, Keys.NumPad7, Keys.D7 : xTargetColumn = niA7 : GoTo MoveToColumn
            Case Keys.Oem8, Keys.NumPad8, Keys.D8 : xTargetColumn = niA8 : GoTo MoveToColumn

MoveToColumn:   If xTargetColumn = -1 Then Exit Select
                If Not nEnabled(xTargetColumn) Then Exit Select

                For xI2 As Integer = 1 To UBound(Notes)
                    If Not Notes(xI2).Selected Then Continue For

                    RedoMoveNote(Notes(xI2), xTargetColumn, Notes(xI2).VPosition, True, xUndo, xRedo)
                    Notes(xI2).ColumnIndex = xTargetColumn
                Next
                AddUndo(xUndo, xBaseRedo.Next)
                UpdatePairing()
                CalculateTotalNotes()
                RefreshPanelAll()

        End Select

        If My.Computer.Keyboard.CtrlKeyDown And (Not My.Computer.Keyboard.AltKeyDown) And (Not My.Computer.Keyboard.ShiftKeyDown) Then
            Select Case e.KeyCode
                Case Keys.Z : TBUndo_Click(TBUndo, New EventArgs)
                Case Keys.Y : TBRedo_Click(TBRedo, New EventArgs)
                Case Keys.X : TBCut_Click(TBCut, New EventArgs)
                Case Keys.C : TBCopy_Click(TBCopy, New EventArgs)
                Case Keys.V : TBPaste_Click(TBPaste, New EventArgs)
                Case Keys.A : mnSelectAll_Click(mnSelectAll, New EventArgs)
                Case Keys.F : TBFind_Click(TBFind, New EventArgs)
                Case Keys.T : TBStatistics_Click(TBStatistics, New EventArgs)
            End Select
        End If

        PMainInMouseMove(sender)
        POStatusRefresh()
    End Sub

    Private Sub PMainInResize(ByVal sender As Object, ByVal e As System.EventArgs) Handles PMainIn.Resize, PMainInL.Resize, PMainInR.Resize
        If Not Me.Created Then Exit Sub

        Dim iI As Integer = sender.Tag
        spWidth(0) = PMainL.Width
        spWidth(1) = PMain.Width
        spWidth(2) = PMainR.Width

        Select Case iI
            Case 0
                VSL.LargeChange = sender.Height * 0.9
                VSL.Maximum = VSL.LargeChange - 1
                HSL.LargeChange = sender.Width / gxWidth
                If HSL.Value > HSL.Maximum - HSL.LargeChange + 1 Then HSL.Value = HSL.Maximum - HSL.LargeChange + 1
            Case 1
                VS.LargeChange = sender.Height * 0.9
                VS.Maximum = VS.LargeChange - 1
                HS.LargeChange = sender.Width / gxWidth
                If HS.Value > HS.Maximum - HS.LargeChange + 1 Then HS.Value = HS.Maximum - HS.LargeChange + 1
            Case 2
                VSR.LargeChange = sender.Height * 0.9
                VSR.Maximum = VSR.LargeChange - 1
                HSR.LargeChange = sender.Width / gxWidth
                If HSR.Value > HSR.Maximum - HSR.LargeChange + 1 Then HSR.Value = HSR.Maximum - HSR.LargeChange + 1
        End Select
        RefreshPanel(iI, sender.DisplayRectangle)
    End Sub

    Private Sub PMainInLostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles PMainIn.LostFocus, PMainInL.LostFocus, PMainInR.LostFocus
        RefreshPanelAll()
    End Sub

    Private Sub PMainInMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PMainIn.MouseDown, PMainInL.MouseDown, PMainInR.MouseDown
        tempFirstMouseDown = FirstClickDisabled And Not sender.Focused

        spFocus = sender.Tag
        sender.Focus()
        pMouseDown = New Point(-1, -1)
        VSValue = spV(spFocus)

        If NTInput Then bAdjustUpper = False : bAdjustLength = False
        Me.ctrlPressed = False : Me.ctrlForDuplicate = False

        If MiddleButtonClicked Then MiddleButtonClicked = False : Exit Sub

        Dim xHS As Long = spH(spFocus)
        Dim xVS As Long = spV(spFocus)
        Dim xHeight As Integer = spMain(spFocus).Height

        Select Case e.Button
            Case Windows.Forms.MouseButtons.Left
                If tempFirstMouseDown And Not TBTimeSelect.Checked Then RefreshPanelAll() : Exit Select

                KMouseOver = -1
                'If K Is Nothing Then pMouseDown = e.Location : Exit Select

                'Find the clicked K
                Dim NoteIndex As Integer = -1
                For xI1 = UBound(Notes) To 0 Step -1
                    'If mouse is clicking on a K
                    If MouseInNote(e, xHS, xVS, xHeight, xI1) Then
                        ' found it!
                        NoteIndex = xI1
                        deltaVPosition = IIf(NTInput, (xHeight - e.Y - gxHeight * xVS - 1) / gxHeight - Notes(xI1).VPosition, 0)

                        If NTInput And My.Computer.Keyboard.ShiftKeyDown Then
                            bAdjustUpper = e.Y <= VerticalPositiontoDisplay(Notes(xI1).VPosition + Notes(xI1).Length, xVS, xHeight)
                            bAdjustLength = e.Y >= VerticalPositiontoDisplay(Notes(xI1).VPosition, xVS, xHeight) - vo.kHeight Or bAdjustUpper
                        End If

                        Exit For

                    End If
                Next

                'Play wav
                If ClickStopPreview Then PreviewNote("", True)
                'My.Computer.Audio.Stop()
                If NoteIndex > 0 And PreviewOnClick AndAlso Not isColumnNumeric(Notes(NoteIndex).ColumnIndex) Then
                    Dim xI2 As Integer = Notes(NoteIndex).Value \ 10000
                    If xI2 <= 0 Then xI2 = 1
                    If xI2 >= 1296 Then xI2 = 1295

                    If Not hWAV(xI2) = "" Then ' AndAlso Path.GetExtension(hWAV(xI2)).ToLower = ".wav" Then
                        Dim xFileLocation As String = IIf(ExcludeFileName(FileName) = "", InitPath, ExcludeFileName(FileName)) & "\" & hWAV(xI2)
                        If Not ClickStopPreview Then PreviewNote("", True)
                        PreviewNote(xFileLocation, False)
                    End If
                End If

                For xI1 = 0 To UBound(Notes)
                    Notes(xI1).TempMouseDown = False
                Next

                If TBSelect.Checked Then

                    If NoteIndex >= 0 And e.Clicks = 2 Then
                        DoubleClickNoteIndex(NoteIndex)
                    ElseIf NoteIndex > 0 Then
                        'KMouseDown = -1
                        ReDim uNotes(-1)

                        'KMouseDown = xITemp
                        Notes(NoteIndex).TempMouseDown = True

                        If My.Computer.Keyboard.CtrlKeyDown Then
                            'If Not K(xITemp).Selected Then K(xITemp).Selected = True
                            ctrlPressed = True

                        Else
                            If Not Notes(NoteIndex).Selected Then
                                For xI1 = 0 To UBound(Notes)
                                    If Notes(xI1).Selected Then Notes(xI1).Selected = False
                                Next
                                Notes(NoteIndex).Selected = True
                            End If

                            Dim xI2 As Integer = 0
                            For xI1 = 0 To UBound(Notes)
                                If Notes(xI1).Selected Then xI2 += 1
                            Next
                            bAdjustSingle = xI2 = 1

                            ReDim uNotes(xI2)
                            uNotes(0) = Notes(NoteIndex)
                            Notes(NoteIndex).TempIndex = 0
                            xI2 = 1

                            For xI1 = 1 To NoteIndex - 1
                                If Notes(xI1).Selected Then Notes(xI1).TempIndex = xI2 : uNotes(xI2) = Notes(xI1) : xI2 += 1
                            Next
                            For xI1 = NoteIndex + 1 To UBound(Notes)
                                If Notes(xI1).Selected Then Notes(xI1).TempIndex = xI2 : uNotes(xI2) = Notes(xI1) : xI2 += 1
                            Next

                            'uCol = RealColumnToEnabled(K(xITemp).ColumnIndex)
                            'uVPos = K(xITemp).VPosition
                            'uNote = K(xITemp)
                            uAdded = False

                        End If

                    Else
                        ReDim uNotes(-1)
                        pMouseDown = e.Location
                        If Not My.Computer.Keyboard.CtrlKeyDown Then
                            For xI1 = 0 To UBound(Notes)
                                Notes(xI1).Selected = False
                                Notes(xI1).TempSelected = False
                            Next
                        Else
                            For xI1 = 0 To UBound(Notes)
                                Notes(xI1).TempSelected = Notes(xI1).Selected
                            Next
                        End If
                    End If

                ElseIf NTInput And TBWrite.Checked Then
                    TempVPosition = -1
                    SelectedColumn = -1
                    TempDraw = False

                    Dim xVPosition As Double = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight 'VPosition of the mouse
                    If gSnap Then xVPosition = SnapToGrid(xVPosition)
                    If xVPosition < 0 Or xVPosition >= VPosition1000() Then Exit Select

                    Dim xColumn = GetColumnAtEvent(e, xHS)

                    For xI2 As Integer = UBound(Notes) To 1 Step -1
                        If Notes(xI2).VPosition = xVPosition And Notes(xI2).ColumnIndex = xColumn Then NoteIndex = xI2 : Exit For
                    Next

                    If NoteIndex > 0 Then
                        ReDim uNotes(0)
                        uNotes(0) = Notes(NoteIndex)
                        Notes(NoteIndex).TempIndex = 0

                        'KMouseDown = xITemp
                        Notes(NoteIndex).TempMouseDown = True
                        Notes(NoteIndex).Length = xVPosition - Notes(NoteIndex).VPosition

                        'uVPos = K(xITemp).VPosition
                        bAdjustUpper = True

                        Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                        Dim xRedo As UndoRedo.LinkedURCmd = Nothing
                        Me.RedoLongNoteModify(uNotes(0), Notes(NoteIndex).VPosition, Notes(NoteIndex).Length, True, xUndo, xRedo)
                        AddUndo(xUndo, xRedo)
                        'With uNote
                        '    AddUndo(sCmdKL(.ColumnIndex, .VPosition, .Value, K(xITemp).Length, .Hidden, .Length, True, True), _
                        '            sCmdKL(.ColumnIndex, .VPosition, .Value, .Length, .Hidden, K(xITemp).Length, True, True))
                        'End With

                    ElseIf isColumnNumeric(xColumn) Then
                        Dim xMessage As String = Strings.Messages.PromptEnterNumeric
                        If xColumn = niBPM Then xMessage = Strings.Messages.PromptEnterBPM
                        If xColumn = niSTOP Then xMessage = Strings.Messages.PromptEnterSTOP

                        Dim xBoolean2 As Boolean = My.Computer.Keyboard.CtrlKeyDown
                        Dim xD1 As Double = Val(InputBox(xMessage, Me.Text)) * 10000

                        If Not xD1 = 0 Then
                            If xD1 <= 0 Then xD1 = 1
                            If xD1 > 655359999 Then xD1 = 655359999

                            Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                            Dim xRedo As UndoRedo.LinkedURCmd = New UndoRedo.Void
                            Dim xBaseRedo As UndoRedo.LinkedURCmd = xRedo

                            For xI1 = 1 To UBound(Notes)
                                If Notes(xI1).VPosition = xVPosition AndAlso Notes(xI1).ColumnIndex = xColumn Then _
                                    Me.RedoRemoveNote(Notes(xI1), True, xUndo, xRedo)
                            Next
                            Me.RedoAddNote(xColumn, xVPosition, xD1, 0, xBoolean2, True, xUndo, xRedo)

                            AddNote(xVPosition, xColumn, xD1, 0, xBoolean2)
                            AddUndo(xUndo, xBaseRedo.Next)
                        End If

                        TempDraw = True

                    Else
                        Dim xLbl As Integer = (LWAV.SelectedIndex + 1) * 10000
                        Dim xBoolean2 As Boolean = My.Computer.Keyboard.CtrlKeyDown

                        ReDim Preserve Notes(UBound(Notes) + 1)
                        With Notes(UBound(Notes))
                            .VPosition = xVPosition
                            .ColumnIndex = xColumn
                            .Value = xLbl
                            .Hidden = xBoolean2
                            .TempMouseDown = True
                        End With

                        ReDim uNotes(0)
                        uNotes(0) = Notes(UBound(Notes))
                        uNotes(0).PairWithI = -1

                        'KMouseDown = 1

                        'uNote.Value = 0
                        'uVPos = xVPosition
                        uAdded = False

                        Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                        Dim xRedo As UndoRedo.LinkedURCmd = Nothing
                        Me.RedoAddNote(Notes(UBound(Notes)), True, xUndo, xRedo)
                        AddUndo(xUndo, xRedo)

                    End If

                    SortByVPositionInsertion()
                    UpdatePairing()
                    CalculateTotalNotes()

                ElseIf TBTimeSelect.Checked Then

                    Dim xL1 As Double
                    If NoteIndex >= 0 Then xL1 = Notes(NoteIndex).VPosition _
                                   Else xL1 = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight

                    vSelAdjust = My.Computer.Keyboard.ShiftKeyDown

                    vSelMouseOverLine = 0
                    If Math.Abs(e.Y - VerticalPositiontoDisplay(vSelStart + vSelLength, xVS, xHeight)) <= vo.PEDeltaMouseOver Then
                        vSelMouseOverLine = 3
                    ElseIf Math.Abs(e.Y - VerticalPositiontoDisplay(vSelStart + vSelHalf, xVS, xHeight)) <= vo.PEDeltaMouseOver Then
                        vSelMouseOverLine = 2
                    ElseIf Math.Abs(e.Y - VerticalPositiontoDisplay(vSelStart, xVS, xHeight)) <= vo.PEDeltaMouseOver Then
                        vSelMouseOverLine = 1
                    End If

                    If Not vSelAdjust Then
                        If vSelMouseOverLine = 1 Then
                            If gSnap And NoteIndex <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then xL1 = SnapToGrid(xL1)
                            vSelLength += vSelStart - xL1
                            vSelHalf += vSelStart - xL1
                            vSelStart = xL1

                        ElseIf vSelMouseOverLine = 2 Then
                            vSelHalf = xL1
                            If gSnap And NoteIndex <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then vSelHalf = SnapToGrid(vSelHalf)
                            vSelHalf -= vSelStart

                        ElseIf vSelMouseOverLine = 3 Then
                            vSelLength = xL1
                            If gSnap And NoteIndex <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then vSelLength = SnapToGrid(vSelLength)
                            vSelLength -= vSelStart

                        Else
                            vSelLength = 0
                            vSelStart = xL1
                            If gSnap And NoteIndex <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then vSelStart = SnapToGrid(vSelStart)
                        End If
                        ValidateSelection()

                    Else
                        If vSelMouseOverLine = 2 Then
                            SortByVPositionInsertion()
                            vSelPStart = vSelStart
                            vSelPLength = vSelLength
                            vSelPHalf = vSelHalf
                            vSelK = Notes
                            ReDim Preserve vSelK(UBound(vSelK))

                            If gSnap And NoteIndex <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then xL1 = SnapToGrid(xL1)
                            AddUndo(New UndoRedo.Void, New UndoRedo.Void)
                            BPMChangeHalf(xL1 - vSelHalf - vSelStart, , True)
                            SortByVPositionInsertion()
                            UpdatePairing()
                            CalculateGreatestVPosition()

                        ElseIf vSelMouseOverLine = 3 Or vSelMouseOverLine = 1 Then
                            SortByVPositionInsertion()
                            vSelPStart = vSelStart
                            vSelPLength = vSelLength
                            vSelPHalf = vSelHalf
                            vSelK = Notes
                            ReDim Preserve vSelK(UBound(vSelK))

                            If gSnap And NoteIndex <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then xL1 = SnapToGrid(xL1)
                            AddUndo(New UndoRedo.Void, New UndoRedo.Void)
                            BPMChangeTop(IIf(vSelMouseOverLine = 3, xL1 - vSelStart, vSelStart + vSelLength - xL1) / vSelLength, , True)
                            SortByVPositionInsertion()
                            UpdatePairing()
                            CalculateGreatestVPosition()

                        Else
                            vSelLength = xL1
                            If gSnap And NoteIndex <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then vSelLength = SnapToGrid(vSelLength)
                            vSelLength -= vSelStart
                        End If

                    End If

                    If vSelLength Then
                        Dim xVLower As Double = IIf(vSelLength > 0, vSelStart, vSelStart + vSelLength)
                        Dim xVUpper As Double = IIf(vSelLength < 0, vSelStart, vSelStart + vSelLength)
                        If NTInput Then
                            For xI2 As Integer = 1 To UBound(Notes)
                                Notes(xI2).Selected = Not Notes(xI2).VPosition >= xVUpper And Not Notes(xI2).VPosition + Notes(xI2).Length < xVLower And nEnabled(Notes(xI2).ColumnIndex)
                            Next
                        Else
                            For xI2 As Integer = 1 To UBound(Notes)
                                Notes(xI2).Selected = Notes(xI2).VPosition >= xVLower And Notes(xI2).VPosition < xVUpper And nEnabled(Notes(xI2).ColumnIndex)
                            Next
                        End If
                    Else
                        For xI2 As Integer = 1 To UBound(Notes)
                            Notes(xI2).Selected = False
                        Next
                    End If

                End If

                RefreshPanelAll()
                POStatusRefresh()

            Case Windows.Forms.MouseButtons.Middle
                If MiddleButtonMoveMethod = 1 Then
                    tempX = e.X
                    tempY = e.Y
                    tempV = xVS
                    tempH = xHS
                Else
                    MiddleButtonLocation = Cursor.Position
                    MiddleButtonClicked = True
                    TimerMiddle.Enabled = True
                End If

            Case Windows.Forms.MouseButtons.Right
                KMouseOver = -1
                'KMouseDown = -1
                ReDim uNotes(-1)
                'If K Is Nothing Then pMouseDown = e.Location : Exit Select

                If tempFirstMouseDown Then GoTo Jump0

                Dim xI1 As Integer
                For xI1 = UBound(Notes) To 1 Step -1
                    'If mouse is clicking on a K
                    If MouseInNote(e, xHS, xVS, xHeight, xI1) Then

                        If My.Computer.Keyboard.ShiftKeyDown Then
                            LWAV.SelectedIndices.Clear()
                            LWAV.SelectedIndex = C36to10(C10to36(Notes(xI1).Value \ 10000)) - 1
                            validate_LWAV_view()

                        Else
                            Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                            Dim xRedo As UndoRedo.LinkedURCmd = Nothing

                            Me.RedoRemoveNote(Notes(xI1), True, xUndo, xRedo)
                            RemoveNote(xI1)

                            AddUndo(xUndo, xRedo)
                            RefreshPanelAll()
                        End If

                        Exit For
                    End If
                Next

                CalculateTotalNotes()

Jump0:          'If xI1 = 0 Then
                '    menuVPosition = -(gxHeight * xVS - xHeight + 1 + e.Y) / gxHeight
                '    Menu1.Show(sender, e.Location) ', ToolStripDropDownDirection.BelowRight)
                '    'mnSys.Show(sender, e.Location, ToolStripDropDownDirection.BelowLeft)
                'End If
        End Select
    End Sub

    Private Sub DoubleClickNoteIndex(NoteIndex As Integer)
        Dim Note As Note = Notes(NoteIndex)
        Dim NoteColumn As Integer = Note.ColumnIndex

        If isColumnNumeric(NoteColumn) Then
            'BPM/Stop prompt
            Dim xMessage As String = Strings.Messages.PromptEnterNumeric
            If NoteColumn = niBPM Then xMessage = Strings.Messages.PromptEnterBPM
            If NoteColumn = niSTOP Then xMessage = Strings.Messages.PromptEnterSTOP

            Dim PromptValue As Double = Val(InputBox(xMessage, Me.Text)) * 10000
            If Not PromptValue = 0 Then
                If PromptValue <= 0 Then PromptValue = 1
                If PromptValue > 655359999 Then PromptValue = 655359999

                Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                Dim xRedo As UndoRedo.LinkedURCmd = Nothing
                RedoRelabelNote(Note, PromptValue, NoteIndex, xUndo, xRedo)
                Note.Value = PromptValue
                If NoteIndex = 0 Then THBPM.Value = PromptValue / 10000
                AddUndo(xUndo, xRedo)
            End If

        Else
            'Label prompt
            Dim xStr As String = UCase(Trim(InputBox(Strings.Messages.PromptEnter, Me.Text)))

            If Len(xStr) = 0 Then Return

            If IsBase36(xStr) And Not (xStr = "00" Or xStr = "0") Then
                Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                Dim xRedo As UndoRedo.LinkedURCmd = Nothing
                RedoRelabelNote(Note, C36to10(xStr) * 10000, True, xUndo, xRedo)
                Note.Value = C36to10(xStr) * 10000
                AddUndo(xUndo, xRedo)
                Return
            Else
                MsgBox(Strings.Messages.InvalidLabel, MsgBoxStyle.Critical, Strings.Messages.Err)
            End If

        End If
    End Sub

    Private Function MouseInNote(e As MouseEventArgs, xHS As Long, xVS As Long, xHeight As Integer, xI1 As Integer) As Boolean
        Return e.X >= HorizontalPositiontoDisplay(nLeft(Notes(xI1).ColumnIndex), xHS) + 1 And
                               e.X <= HorizontalPositiontoDisplay(nLeft(Notes(xI1).ColumnIndex) + nLength(Notes(xI1).ColumnIndex), xHS) - 1 And
                               e.Y >= VerticalPositiontoDisplay(Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0), xVS, xHeight) - vo.kHeight And
                               e.Y <= VerticalPositiontoDisplay(Notes(xI1).VPosition, xVS, xHeight)
    End Function

    Private Sub PMainInMouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles PMainIn.MouseEnter, PMainInL.MouseEnter, PMainInR.MouseEnter
        spMouseOver = sender.Tag
        Dim xPMainIn As Panel = sender
        If AutoFocusMouseEnter AndAlso Me.Focused Then xPMainIn.Focus() : spFocus = spMouseOver
        If FirstMouseEnter Then FirstMouseEnter = False : xPMainIn.Focus() : spFocus = spMouseOver
    End Sub

    Private Sub PMainInMouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles PMainIn.MouseLeave, PMainInL.MouseLeave, PMainInR.MouseLeave
        KMouseOver = -1
        'KMouseDown = -1
        ReDim uNotes(-1)
        TempVPosition = -1
        SelectedColumn = -1
        RefreshPanelAll()
    End Sub

    Private Sub PMainInMouseMove(ByVal sender As Panel)
        Dim p As Point = sender.PointToClient(Cursor.Position)
        Me.PMainInMouseMove(sender, New MouseEventArgs(Windows.Forms.MouseButtons.None, 0, p.X, p.Y, 0))
    End Sub

    Private Sub PMainInMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PMainIn.MouseMove, PMainInL.MouseMove, PMainInR.MouseMove
        MouseMoveStatus = e.Location

        Dim iI As Integer = sender.Tag

        Dim xHS As Long = spH(iI)
        Dim xVS As Long = spV(iI)
        Dim xHeight As Integer = spMain(iI).Height
        Dim xWidth As Integer = spMain(iI).Width

        Select Case e.Button
            Case Windows.Forms.MouseButtons.None
                'If K Is Nothing Then Exit Select
                If MiddleButtonClicked Then Exit Select

                If isFullScreen Then
                    If e.Y < 5 Then ToolStripContainer1.TopToolStripPanelVisible = True Else ToolStripContainer1.TopToolStripPanelVisible = False
                End If

                Dim xMouseRemainInSameRegion As Boolean = False

                Dim noteIndex As Integer
                Dim foundNoteIndex As Integer = -1
                For noteIndex = UBound(Notes) To 0 Step -1
                    If MouseInNote(e, xHS, xVS, xHeight, noteIndex) Then
                        foundNoteIndex = noteIndex

                        xMouseRemainInSameRegion = foundNoteIndex = KMouseOver
                        If NTInput Then
                            Dim xbAdjustUpper As Boolean = (e.Y <= VerticalPositiontoDisplay(Notes(noteIndex).VPosition + Notes(noteIndex).Length, xVS, xHeight)) And My.Computer.Keyboard.ShiftKeyDown
                            Dim xbAdjustLength As Boolean = (e.Y >= VerticalPositiontoDisplay(Notes(noteIndex).VPosition, xVS, xHeight) - vo.kHeight Or xbAdjustUpper) And My.Computer.Keyboard.ShiftKeyDown
                            xMouseRemainInSameRegion = xMouseRemainInSameRegion And xbAdjustUpper = bAdjustUpper And xbAdjustLength = bAdjustLength
                            bAdjustUpper = xbAdjustUpper
                            bAdjustLength = xbAdjustLength
                        End If

                        Exit For
                    End If
                Next

                Dim xTempbTimeSelectionMode As Boolean = TBTimeSelect.Checked

                If TBSelect.Checked Or xTempbTimeSelectionMode Then

                    If xMouseRemainInSameRegion Then Exit Select
                    If KMouseOver >= 0 Then KMouseOver = -1 : RefreshPanelAll()

                    If xTempbTimeSelectionMode Then

                        Dim xMouseOverLine As Integer = vSelMouseOverLine
                        vSelMouseOverLine = 0

                        If Math.Abs(e.Y - VerticalPositiontoDisplay(vSelStart + vSelLength, xVS, xHeight)) <= vo.PEDeltaMouseOver Then
                            vSelMouseOverLine = 3
                        ElseIf Math.Abs(e.Y - VerticalPositiontoDisplay(vSelStart + vSelHalf, xVS, xHeight)) <= vo.PEDeltaMouseOver Then
                            vSelMouseOverLine = 2
                        ElseIf Math.Abs(e.Y - VerticalPositiontoDisplay(vSelStart, xVS, xHeight)) <= vo.PEDeltaMouseOver Then
                            vSelMouseOverLine = 1
                        End If

                        If xMouseOverLine <> vSelMouseOverLine Then RefreshPanelAll()
                    End If

                    If foundNoteIndex > -1 Then
                        Dim xDispX As Integer = HorizontalPositiontoDisplay(nLeft(Notes(foundNoteIndex).ColumnIndex), xHS)
                        Dim xDispY As Integer = IIf(Not NTInput Or (bAdjustLength And Not bAdjustUpper),
                                                    VerticalPositiontoDisplay(Notes(foundNoteIndex).VPosition, xVS, xHeight) - vo.kHeight - 1,
                                                    VerticalPositiontoDisplay(Notes(foundNoteIndex).VPosition + Notes(foundNoteIndex).Length, xVS, xHeight) - vo.kHeight - 1)
                        Dim xDispW As Integer = nLength(Notes(foundNoteIndex).ColumnIndex) * gxWidth + 1
                        Dim xDispH As Integer = IIf(Not NTInput Or bAdjustLength,
                                                    vo.kHeight + 3,
                                                    Notes(foundNoteIndex).Length * gxHeight + vo.kHeight + 3)

                        Dim e1 As BufferedGraphics = BufferedGraphicsManager.Current.Allocate(spMain(iI).CreateGraphics, New Rectangle(xDispX, xDispY, xDispW, xDispH))
                        e1.Graphics.FillRectangle(vo.Bg, New Rectangle(xDispX, xDispY, xDispW, xDispH))

                        If NTInput Then DrawNoteNT(Notes(foundNoteIndex), e1, xHS, xVS, xHeight) Else DrawNote(Notes(foundNoteIndex), e1, xHS, xVS, xHeight)

                        e1.Graphics.DrawRectangle(IIf(bAdjustLength, vo.kMouseOverE, vo.kMouseOver), xDispX, xDispY, xDispW - 1, xDispH - 1)

                        e1.Render(spMain(iI).CreateGraphics)
                        e1.Dispose()
                    End If

                    KMouseOver = foundNoteIndex

                ElseIf TBWrite.Checked Then
                    TempVPosition = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight 'VPosition of the mouse
                    If gSnap Then TempVPosition = SnapToGrid(TempVPosition)

                    SelectedColumn = GetColumnAtEvent(e, xHS)  'get the enabled column where mouse is 

                    TempLength = 0
                    If foundNoteIndex > -1 Then TempLength = Notes(foundNoteIndex).Length

                    RefreshPanelAll()
                End If

            Case Windows.Forms.MouseButtons.Left
                If tempFirstMouseDown And Not TBTimeSelect.Checked Then Exit Select

                tempX = 0
                tempY = 0
                If e.X < 0 Or e.X > xWidth Or e.Y < 0 Or e.Y > xHeight Then
                    If e.X < 0 Then tempX = e.X
                    If e.X > xWidth Then tempX = e.X - xWidth
                    If e.Y < 0 Then tempY = e.Y
                    If e.Y > xHeight Then tempY = e.Y - xHeight
                    Timer1.Enabled = True
                Else
                    Timer1.Enabled = False
                End If

                If TBSelect.Checked Then

                    pMouseMove = e.Location

                    'If K Is Nothing Then RefreshPanelAll() : Exit Select

                    If Not pMouseDown = New Point(-1, -1) Then
                        Dim rSBox As New Rectangle(IIf(pMouseMove.X > pMouseDown.X, pMouseDown.X, pMouseMove.X),
                                                   IIf(pMouseMove.Y > pMouseDown.Y, pMouseDown.Y, pMouseMove.Y),
                                                   Math.Abs(pMouseMove.X - pMouseDown.X),
                                                   Math.Abs(pMouseMove.Y - pMouseDown.Y))
                        Dim rNote As Rectangle

                        Dim xI1 As Integer
                        For xI1 = 1 To UBound(Notes)
                            rNote = New Rectangle(HorizontalPositiontoDisplay(nLeft(Notes(xI1).ColumnIndex), xHS) + 1,
                                                  VerticalPositiontoDisplay(Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0), xVS, xHeight) - vo.kHeight,
                                                  nLength(Notes(xI1).ColumnIndex) * gxWidth - 2,
                                                  vo.kHeight + IIf(NTInput, Notes(xI1).Length * gxHeight, 0))
                            If Math.Abs((rSBox.X + rSBox.Width / 2) - (rNote.X + rNote.Width / 2)) <= Math.Abs((rSBox.Width + rNote.Width) / 2) And
                               Math.Abs((rSBox.Y + rSBox.Height / 2) - (rNote.Y + rNote.Height / 2)) <= Math.Abs((rSBox.Height + rNote.Height) / 2) Then
                                Notes(xI1).Selected = Not Notes(xI1).TempSelected And nEnabled(Notes(xI1).ColumnIndex)
                            Else
                                Notes(xI1).Selected = Notes(xI1).TempSelected And nEnabled(Notes(xI1).ColumnIndex)
                            End If
                        Next

                        'ElseIf Not KMouseDown = -1 Then
                    ElseIf uNotes.Length <> 0 Then
                        Dim dColumn As Integer = 0
                        Dim dVPosition As Double
                        Dim mouseColumn As Integer 'Enabled
                        Dim mouseVPosition As Double
                        Dim xI1 As Integer

                        Dim xITemp As Integer
                        For xI1 = 1 To UBound(Notes)
                            If Notes(xI1).TempMouseDown Then xITemp = xI1 : Exit For
                        Next

                        mouseVPosition = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight - IIf(Not bAdjustLength, deltaVPosition, 0)  'VPosition of the mouse
                        If gSnap Then mouseVPosition = SnapToGrid(mouseVPosition)

                        If bAdjustLength And bAdjustSingle Then
                            If bAdjustUpper AndAlso mouseVPosition < Notes(xITemp).VPosition Then
                                bAdjustUpper = False
                                Notes(xITemp).VPosition += Notes(xITemp).Length
                                Notes(xITemp).Length *= -1
                            ElseIf Not bAdjustUpper AndAlso mouseVPosition > Notes(xITemp).VPosition + Notes(xITemp).Length Then
                                bAdjustUpper = True
                                Notes(xITemp).VPosition += Notes(xITemp).Length
                                Notes(xITemp).Length *= -1
                            End If
                        End If

                        'If moving
                        If Not bAdjustLength Then
                            If DisableVerticalMove Then mouseVPosition = uNotes(0).VPosition
                            dVPosition = mouseVPosition - Notes(xITemp).VPosition  'delta VPosition

                            xI1 = 0
                            Dim mLeft As Integer = e.X / gxWidth + xHS 'horizontal position of the mouse
                            If mLeft >= 0 Then
                                Do
                                    If mLeft < nLeft(xI1 + 1) Or xI1 >= gColumns Then mouseColumn = RealColumnToEnabled(xI1) : Exit Do 'get the column where mouse is 
                                    xI1 += 1
                                Loop
                            End If

                            dColumn = mouseColumn - RealColumnToEnabled(Notes(xITemp).ColumnIndex) 'get the enabled delta column where mouse is 

                            dColumn = mouseColumn - RealColumnToEnabled(Notes(xITemp).ColumnIndex) 'get the enabled delta column where mouse is 

                            'Ks cannot be beyond the left, the upper and the lower boundary
                            mLeft = 0
                            Dim mVPosition As Double = 0
                            Dim muVPosition As Double = 191999
                            For xI1 = 1 To UBound(Notes)
                                If Notes(xI1).Selected Then
                                    mLeft = IIf(RealColumnToEnabled(Notes(xI1).ColumnIndex) + dColumn < mLeft,
                                                RealColumnToEnabled(Notes(xI1).ColumnIndex) + dColumn,
                                                mLeft)
                                    mVPosition = IIf(Notes(xI1).VPosition + dVPosition < mVPosition,
                                                     Notes(xI1).VPosition + dVPosition,
                                                     mVPosition)
                                    muVPosition = IIf(Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0) + dVPosition > muVPosition,
                                                      Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0) + dVPosition,
                                                      muVPosition)
                                End If
                            Next
                            muVPosition -= 191999

                            Dim xCol As Integer
                            Dim xVPos As Double

                            Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                            Dim xRedo As UndoRedo.LinkedURCmd = New UndoRedo.Void
                            Dim xBaseRedo As UndoRedo.LinkedURCmd = xRedo

                            'start moving
                            For xI1 = 1 To UBound(Notes)
                                If Not Notes(xI1).Selected Then Continue For

                                xCol = EnabledColumnToReal(RealColumnToEnabled(Notes(xI1).ColumnIndex) + dColumn - mLeft)
                                xVPos = Notes(xI1).VPosition + dVPosition - mVPosition - muVPosition
                                Me.RedoMoveNote(uNotes(Notes(xI1).TempIndex), xCol, xVPos, True, xUndo, xRedo)

                                Notes(xI1).ColumnIndex = xCol
                                Notes(xI1).VPosition = xVPos
                            Next

                            'If mouseColumn - uNotes(0).ColumnIndex - mLeft <> 0 Or mouseVPosition - uNotes(0).VPosition - mVPosition - muVPosition <> 0 Then
                            AddUndo(xUndo, xBaseRedo.Next, uAdded)
                            If Not uAdded Then uAdded = True

                            'End If

                        ElseIf bAdjustUpper Then    'If adjusting upper end
                            dVPosition = mouseVPosition - Notes(xITemp).VPosition - Notes(xITemp).Length  'delta Length
                            '< 0 means shorten, > 0 means lengthen

                            Dim minLength As Double = 0
                            Dim maxHeight As Double = 191999
                            For xI1 = 1 To UBound(Notes)
                                If Not Notes(xI1).Selected Then Continue For
                                If Notes(xI1).Length + dVPosition < minLength Then minLength = Notes(xI1).Length + dVPosition
                                If Notes(xI1).Length + Notes(xI1).VPosition + dVPosition > maxHeight Then maxHeight = Notes(xI1).Length + Notes(xI1).VPosition + dVPosition
                            Next
                            maxHeight -= 191999

                            'declare undo variables
                            Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                            Dim xRedo As UndoRedo.LinkedURCmd = New UndoRedo.Void
                            Dim xBaseRedo As UndoRedo.LinkedURCmd = xRedo

                            'start moving
                            Dim xLen As Double
                            For xI1 = 1 To UBound(Notes)
                                If Not Notes(xI1).Selected Then Continue For

                                xLen = Notes(xI1).Length + dVPosition - minLength - maxHeight
                                Me.RedoLongNoteModify(uNotes(Notes(xI1).TempIndex), Notes(xI1).VPosition, xLen, True, xUndo, xRedo)

                                Notes(xI1).Length = xLen
                            Next

                            'Add undo
                            If dVPosition - minLength - maxHeight <> 0 Then
                                AddUndo(xUndo, xBaseRedo.Next, uAdded)
                                If Not uAdded Then uAdded = True
                            End If

                        Else    'If adjusting lower end
                            dVPosition = mouseVPosition - Notes(xITemp).VPosition  'delta VPosition
                            '> 0 means shorten, < 0 means lengthen

                            Dim minLength As Double = 0
                            Dim minVPosition As Double = 0
                            For xI1 = 1 To UBound(Notes)
                                If Notes(xI1).Selected AndAlso Notes(xI1).Length - dVPosition < minLength Then minLength = Notes(xI1).Length - dVPosition
                                If Notes(xI1).Selected AndAlso Notes(xI1).VPosition + dVPosition < minVPosition Then minVPosition = Notes(xI1).VPosition + dVPosition
                            Next

                            'declare undo variables
                            Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                            Dim xRedo As UndoRedo.LinkedURCmd = New UndoRedo.Void
                            Dim xBaseRedo As UndoRedo.LinkedURCmd = xRedo

                            'start moving
                            Dim xVPos As Double
                            Dim xLen As Double
                            For xI1 = 0 To UBound(Notes)
                                If Not Notes(xI1).Selected Then Continue For

                                xVPos = Notes(xI1).VPosition + dVPosition + minLength - minVPosition
                                xLen = Notes(xI1).Length - dVPosition - minLength + minVPosition
                                Me.RedoLongNoteModify(uNotes(Notes(xI1).TempIndex), xVPos, xLen, True, xUndo, xRedo)

                                Notes(xI1).VPosition = xVPos
                                Notes(xI1).Length = xLen
                            Next

                            'Add undo
                            If dVPosition + minLength - minVPosition <> 0 Then
                                AddUndo(xUndo, xBaseRedo.Next, uAdded)
                                If Not uAdded Then uAdded = True
                            End If

                        End If

                        SortByVPositionInsertion()
                        UpdatePairing()
                        CalculateTotalNotes()
                        'Label1.Text = KInfo(KMouseDown)

                    ElseIf ctrlPressed Then
                        Dim xITemp As Integer
                        For xITemp = 1 To UBound(Notes)
                            If Notes(xITemp).TempMouseDown Then Exit For
                        Next

                        Dim mouseVPosition As Double = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight - deltaVPosition
                        If gSnap Then mouseVPosition = SnapToGrid(mouseVPosition)
                        If DisableVerticalMove Then mouseVPosition = Notes(xITemp).VPosition

                        Dim dVPosition As Double = mouseVPosition - Notes(xITemp).VPosition  'delta VPosition

                        Dim dColumn As Integer = GetColumnAtEvent(e, xHS) - RealColumnToEnabled(Notes(xITemp).ColumnIndex) 'delta Column

                        'Ks cannot be beyond the left, the upper and the lower boundary
                        Dim mLeft = 0
                        Dim mVPosition As Double = 0
                        Dim muVPosition As Double = 191999
                        For xI1 = 1 To UBound(Notes)
                            If Not Notes(xI1).Selected Then Continue For

                            If RealColumnToEnabled(Notes(xI1).ColumnIndex) + dColumn < mLeft Then _
                                mLeft = RealColumnToEnabled(Notes(xI1).ColumnIndex) + dColumn
                            If Notes(xI1).VPosition + dVPosition < mVPosition Then _
                                mVPosition = Notes(xI1).VPosition + dVPosition
                            If Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0) + dVPosition > muVPosition Then _
                                muVPosition = Notes(xI1).VPosition + IIf(NTInput, Notes(xI1).Length, 0) + dVPosition
                        Next
                        muVPosition -= 191999

                        'If not moving then exit
                        If (Not ctrlForDuplicate) And dColumn - mLeft = 0 And dVPosition - mVPosition - muVPosition = 0 Then _
                            GoTo EndCtrlOpn

                        Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                        Dim xRedo As UndoRedo.LinkedURCmd = New UndoRedo.Void
                        Dim xBaseRedo As UndoRedo.LinkedURCmd = xRedo

                        If Not ctrlForDuplicate Then     'If uAdded = False
                            Notes(xITemp).Selected = True

                            Dim xSelectedNotesCount As Integer = 0
                            For i As Integer = 1 To UBound(Notes)
                                If Notes(i).Selected Then xSelectedNotesCount += 1
                            Next

                            Dim xTempNotes(xSelectedNotesCount - 1) As Note
                            Dim xI2 As Integer = 0
                            For i As Integer = 1 To UBound(Notes)
                                If Not Notes(i).Selected Then Continue For

                                xTempNotes(xI2) = Notes(i)
                                xTempNotes(xI2).ColumnIndex = EnabledColumnToReal(RealColumnToEnabled(Notes(i).ColumnIndex) + dColumn - mLeft)
                                xTempNotes(xI2).VPosition = Notes(i).VPosition + dVPosition - mVPosition - muVPosition
                                Me.RedoAddNote(xTempNotes(xI2), True, xUndo, xRedo)

                                Notes(i).Selected = False
                                xI2 += 1
                            Next
                            Notes(xITemp).TempMouseDown = False

                            'copy to K
                            Dim xOrigUBound As Integer = UBound(Notes)
                            ReDim Preserve Notes(xOrigUBound + xSelectedNotesCount)
                            xI2 = 0
                            For i As Integer = xOrigUBound + 1 To UBound(Notes)
                                Notes(i) = xTempNotes(xI2)
                                xI2 += 1
                            Next

                            AddUndo(xUndo, xBaseRedo.Next)
                            ctrlForDuplicate = True

                        Else
                            For i As Integer = 1 To UBound(Notes)
                                If Not Notes(i).Selected Then Continue For

                                Notes(i).ColumnIndex = EnabledColumnToReal(RealColumnToEnabled(Notes(i).ColumnIndex) + dColumn - mLeft)
                                Notes(i).VPosition = Notes(i).VPosition + dVPosition - mVPosition - muVPosition
                                Me.RedoAddNote(Notes(i), True, xUndo, xRedo)
                            Next

                            AddUndo(xUndo, xBaseRedo.Next, True)
                        End If

                        SortByVPositionInsertion()
                        UpdatePairing()
                        CalculateTotalNotes()
EndCtrlOpn:         End If

                ElseIf TBWrite.Checked Then

                    If NTInput Then
                        'If Not KMouseDown = -1 Then
                        If uNotes.Length <> 0 Then

                            Dim xI1 As Integer
                            Dim xITemp As Integer
                            For xI1 = 1 To UBound(Notes)
                                If Notes(xI1).TempMouseDown Then xITemp = xI1 : Exit For
                            Next

                            Dim mouseVPosition As Double = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight  'VPosition of the mouse
                            If gSnap Then mouseVPosition = SnapToGrid(mouseVPosition)

                            With Notes(xITemp)
                                If bAdjustUpper AndAlso mouseVPosition < .VPosition Then
                                    bAdjustUpper = False
                                    .VPosition += .Length
                                    .Length *= -1
                                ElseIf Not bAdjustUpper AndAlso mouseVPosition > .VPosition + .Length Then
                                    bAdjustUpper = True
                                    .VPosition += .Length
                                    .Length *= -1
                                End If

                                If bAdjustUpper Then
                                    .Length = mouseVPosition - .VPosition
                                Else
                                    .Length = .VPosition + .Length - mouseVPosition
                                    .VPosition = mouseVPosition
                                End If

                                If .VPosition < 0 Then .Length += .VPosition : .VPosition = 0
                                If .VPosition + .Length >= VPosition1000() Then .Length = VPosition1000() - 1 - .VPosition

                                If uNotes(0).PairWithI = -1 Then 'If new note
                                    Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                                    Dim xRedo As UndoRedo.LinkedURCmd = Nothing
                                    Me.RedoAddNote(Notes(xITemp), True, xUndo, xRedo)
                                    AddUndo(xUndo, xRedo, True)

                                Else 'If existing note
                                    Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                                    Dim xRedo As UndoRedo.LinkedURCmd = Nothing
                                    Me.RedoLongNoteModify(uNotes(0), .VPosition, .Length, True, xUndo, xRedo)
                                    AddUndo(xUndo, xRedo, True)
                                    'AddUndo(sCmdKC(.ColumnIndex, .VPosition, .Value, .Length, .Hidden, 0, uNote.VPosition - .VPosition, .Value, uNote.Length, .Hidden, True), _
                                    '        sCmdKC(.ColumnIndex, uNote.VPosition, .Value, uNote.Length, .Hidden, 0, .VPosition - uNote.VPosition, .Value, .Length, .Hidden, True), _
                                    '        True)
                                End If

                                SelectedColumn = .ColumnIndex
                                TempVPosition = mouseVPosition
                                TempLength = .Length

                            End With

                            SortByVPositionInsertion()
                            UpdatePairing()
                            CalculateTotalNotes()

                        End If

                    Else
                        TempVPosition = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight 'VPosition of the mouse
                        If gSnap Then TempVPosition = SnapToGrid(TempVPosition)
                        SelectedColumn = GetColumnAtEvent(e, xHS)  'get the enabled column where mouse is 

                    End If

                ElseIf TBTimeSelect.Checked Then
                    Dim xI1 As Integer
                    Dim xITemp As Integer = -1
                    If Notes IsNot Nothing Then
                        For xI1 = UBound(Notes) To 0 Step -1
                            If e.X >= HorizontalPositiontoDisplay(nLeft(Notes(xI1).ColumnIndex), xHS) + 1 And
                               e.X <= HorizontalPositiontoDisplay(nLeft(Notes(xI1).ColumnIndex) + nLength(Notes(xI1).ColumnIndex), xHS) - 1 And
                               e.Y >= VerticalPositiontoDisplay(Notes(xI1).VPosition, xVS, xHeight) - vo.kHeight And
                               e.Y <= VerticalPositiontoDisplay(Notes(xI1).VPosition, xVS, xHeight) Then
                                xITemp = xI1
                                Exit For
                            End If
                        Next
                    End If

                    If Not vSelAdjust Then
                        If vSelMouseOverLine = 1 Then
                            Dim xV As Double = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight
                            If xITemp >= 0 Then xV = Notes(xITemp).VPosition
                            If gSnap And xITemp <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then xV = SnapToGrid(xV)
                            vSelLength += vSelStart - xV
                            vSelHalf += vSelStart - xV
                            vSelStart = xV

                        ElseIf vSelMouseOverLine = 2 Then
                            vSelHalf = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight
                            If xITemp >= 0 Then vSelHalf = Notes(xITemp).VPosition
                            If gSnap And xITemp <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then vSelHalf = SnapToGrid(vSelHalf)
                            vSelHalf -= vSelStart

                        ElseIf vSelMouseOverLine = 3 Then
                            vSelLength = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight
                            If xITemp >= 0 Then vSelLength = Notes(xITemp).VPosition
                            If gSnap And xITemp <= 0 And Not My.Computer.Keyboard.CtrlKeyDown Then vSelLength = SnapToGrid(vSelLength)
                            vSelLength -= vSelStart

                        Else
                            If xITemp >= 0 Then
                                vSelLength = Notes(xITemp).VPosition
                            Else
                                vSelLength = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight
                                If gSnap And Not My.Computer.Keyboard.CtrlKeyDown Then vSelLength = SnapToGrid(vSelLength)
                            End If
                            vSelLength -= vSelStart
                            vSelHalf = vSelLength / 2
                        End If
                        ValidateSelection()

                    Else
                        Dim xL1 As Double = (xHeight - xVS * gxHeight - e.Y - 1) / gxHeight

                        If vSelMouseOverLine = 2 Then
                            vSelStart = vSelPStart
                            vSelLength = vSelPLength
                            vSelHalf = vSelPHalf
                            Notes = vSelK
                            ReDim Preserve Notes(UBound(Notes))

                            If gSnap And Not My.Computer.Keyboard.CtrlKeyDown Then xL1 = SnapToGrid(xL1)
                            BPMChangeHalf(xL1 - vSelHalf - vSelStart, , True)
                            SortByVPositionInsertion()
                            UpdatePairing()
                            CalculateGreatestVPosition()

                        ElseIf vSelMouseOverLine = 3 Or vSelMouseOverLine = 1 Then
                            vSelStart = vSelPStart
                            vSelLength = vSelPLength
                            vSelHalf = vSelPHalf
                            Notes = vSelK
                            ReDim Preserve Notes(UBound(Notes))

                            If gSnap And Not My.Computer.Keyboard.CtrlKeyDown Then xL1 = SnapToGrid(xL1)
                            BPMChangeTop(IIf(vSelMouseOverLine = 3, xL1 - vSelStart, vSelStart + vSelLength - xL1) / vSelLength, , True)
                            SortByVPositionInsertion()
                            UpdatePairing()
                            CalculateGreatestVPosition()

                        Else
                            vSelLength = xL1
                            If gSnap And Not My.Computer.Keyboard.CtrlKeyDown Then vSelLength = SnapToGrid(vSelLength)
                            If xITemp >= 0 Then vSelLength = Notes(xITemp).VPosition
                            vSelLength -= vSelStart
                            ValidateSelection()
                        End If
                    End If

                    If vSelLength Then
                        Dim xVLower As Double = IIf(vSelLength > 0, vSelStart, vSelStart + vSelLength)
                        Dim xVUpper As Double = IIf(vSelLength < 0, vSelStart, vSelStart + vSelLength)
                        If NTInput Then
                            For xI2 As Integer = 1 To UBound(Notes)
                                Notes(xI2).Selected = Notes(xI2).VPosition < xVUpper And Notes(xI2).VPosition + Notes(xI2).Length >= xVLower And nEnabled(Notes(xI2).ColumnIndex)
                            Next
                        Else
                            For xI2 As Integer = 1 To UBound(Notes)
                                Notes(xI2).Selected = Notes(xI2).VPosition >= xVLower And Notes(xI2).VPosition < xVUpper And nEnabled(Notes(xI2).ColumnIndex)
                            Next
                        End If
                    Else
                        For xI2 As Integer = 1 To UBound(Notes)
                            Notes(xI2).Selected = False
                        Next
                    End If

                End If
                RefreshPanelAll()

            Case Windows.Forms.MouseButtons.Middle

                If MiddleButtonMoveMethod = 1 Then
                    Dim xI1 As Integer = tempV + (tempY - e.Y) / gxHeight
                    Dim xI2 As Integer = tempH + (tempX - e.X) / gxWidth
                    If xI1 > 0 Then xI1 = 0
                    If xI2 < 0 Then xI2 = 0

                    Select Case spFocus
                        Case 0
                            If xI1 < VSL.Minimum Then xI1 = VSL.Minimum
                            VSL.Value = xI1

                            If xI2 > HSL.Maximum - HSL.LargeChange + 1 Then xI2 = HSL.Maximum - HSL.LargeChange + 1
                            HSL.Value = xI2

                        Case 1
                            If xI1 < VS.Minimum Then xI1 = VS.Minimum
                            VS.Value = xI1

                            If xI2 > HS.Maximum - HS.LargeChange + 1 Then xI2 = HS.Maximum - HS.LargeChange + 1
                            HS.Value = xI2

                        Case 2
                            If xI1 < VSR.Minimum Then xI1 = VSR.Minimum
                            VSR.Value = xI1

                            If xI2 > HSR.Maximum - HSR.LargeChange + 1 Then xI2 = HSR.Maximum - HSR.LargeChange + 1
                            HSR.Value = xI2

                    End Select
                End If
        End Select
        POStatusRefresh()
    End Sub


    Private Function GetColumnAtEvent(e As MouseEventArgs, xHS As Integer)
        Dim xI1 As Integer = 0
        Dim mLeft As Integer = e.X / gxWidth + xHS 'horizontal position of the mouse
        Dim xColumn = 0
        If mLeft >= 0 Then
            Do
                If mLeft < nLeft(xI1 + 1) Or xI1 >= gColumns Then xColumn = xI1 : Exit Do 'get the column where mouse is 
                xI1 += 1
            Loop
        End If

        Return EnabledColumnToReal(RealColumnToEnabled(xColumn))  'get the enabled column where mouse is 
    End Function


    Private Sub PMainInMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PMainIn.MouseUp, PMainInL.MouseUp, PMainInR.MouseUp
        tempX = 0
        tempY = 0
        tempV = 0
        tempH = 0
        VSValue = -1
        HSValue = -1
        Timer1.Enabled = False
        'KMouseDown = -1
        ReDim uNotes(-1)

        Dim iI As Integer = sender.Tag

        If MiddleButtonClicked AndAlso e.Button = Windows.Forms.MouseButtons.Middle AndAlso
            (MiddleButtonLocation.X - Cursor.Position.X) ^ 2 + (MiddleButtonLocation.Y - Cursor.Position.Y) ^ 2 >= vo.MiddleDeltaRelease Then
            MiddleButtonClicked = False
        End If

        If TBSelect.Checked Then
            pMouseDown = New Point(-1, -1)
            pMouseMove = New Point(-1, -1)

            If ctrlPressed And Not ctrlForDuplicate Then
                For i As Integer = 1 To UBound(Notes)
                    If Notes(i).TempMouseDown Then Notes(i).Selected = Not Notes(i).Selected : Exit For
                Next
            End If

            ctrlPressed = False
            ctrlForDuplicate = False

        ElseIf TBWrite.Checked Then

            If NTInput Then GoTo Jump0
            If tempFirstMouseDown Then GoTo Jump0


            Dim xVPosition As Double


            xVPosition = (sender.Height - spV(iI) * gxHeight - e.Y - 1) / gxHeight 'VPosition of the mouse
            If gSnap Then xVPosition = SnapToGrid(xVPosition)

            Dim xColumn = GetColumnAtEvent(e, spH(iI))

            If e.Button = Windows.Forms.MouseButtons.Left Then
                Dim LongNote As Boolean = My.Computer.Keyboard.ShiftKeyDown
                Dim HiddenNote As Boolean = My.Computer.Keyboard.CtrlKeyDown

                Dim xUndo As UndoRedo.LinkedURCmd = Nothing
                Dim xRedo As UndoRedo.LinkedURCmd = New UndoRedo.Void
                Dim xBaseRedo As UndoRedo.LinkedURCmd = xRedo

                If isColumnNumeric(xColumn) Then
                    Dim xMessage As String = Strings.Messages.PromptEnterNumeric
                    If xColumn = niBPM Then xMessage = Strings.Messages.PromptEnterBPM
                    If xColumn = niSTOP Then xMessage = Strings.Messages.PromptEnterSTOP

                    Dim xD1 As Double = Val(InputBox(xMessage, Me.Text)) * 10000

                    If xD1 = 0 Then GoTo Jump0

                    If xD1 <= 0 Then xD1 = 1
                    If xD1 > 655359999 Then xD1 = 655359999

                    For xI1 = 1 To UBound(Notes)
                        If Notes(xI1).VPosition = xVPosition AndAlso Notes(xI1).ColumnIndex = xColumn Then _
                            RedoRemoveNote(Notes(xI1), True, xUndo, xRedo)
                    Next

                    RedoAddNote(xColumn, xVPosition, xD1, LongNote, HiddenNote, True, xUndo, xRedo)
                    AddNote(xVPosition, xColumn, xD1, LongNote, HiddenNote)

                    AddUndo(xUndo, xBaseRedo.Next)

                Else
                    Dim xUI1 As Integer = (LWAV.SelectedIndex + 1) * 10000

                    For xI1 = 1 To UBound(Notes)
                        If Notes(xI1).VPosition = xVPosition AndAlso Notes(xI1).ColumnIndex = xColumn Then _
                            RedoRemoveNote(Notes(xI1), True, xUndo, xRedo)
                    Next

                    RedoAddNote(xColumn, xVPosition, xUI1, LongNote, HiddenNote, True, xUndo, xRedo)
                    AddNote(xVPosition, xColumn, xUI1, LongNote, HiddenNote)

                    AddUndo(xUndo, xRedo)
                End If
            End If

Jump0:      If Not TempDraw Then TempDraw = True
            TempVPosition = -1
            SelectedColumn = -1
        End If
        CalculateGreatestVPosition()
        RefreshPanelAll()
    End Sub

    Private Sub PMainInMouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PMainIn.MouseWheel, PMainInL.MouseWheel, PMainInR.MouseWheel
        If MiddleButtonClicked Then MiddleButtonClicked = False

        Dim xI1 As Integer

        Select Case spMouseOver
            Case 0
                'xI1 = spV(iI) - Math.Sign(e.Delta) * VSL.SmallChange * 5 / gxHeight
                xI1 = spV(spMouseOver) - Math.Sign(e.Delta) * gWheel
                If xI1 > 0 Then xI1 = 0
                If xI1 < VSL.Minimum Then xI1 = VSL.Minimum
                VSL.Value = xI1
            Case 1
                'xI1 = spV(iI) - Math.Sign(e.Delta) * VS.SmallChange * 5 / gxHeight
                xI1 = spV(spMouseOver) - Math.Sign(e.Delta) * gWheel
                If xI1 > 0 Then xI1 = 0
                If xI1 < VS.Minimum Then xI1 = VS.Minimum
                VS.Value = xI1
            Case 2
                'xI1 = spV(iI) - Math.Sign(e.Delta) * VSR.SmallChange * 5 / gxHeight
                xI1 = spV(spMouseOver) - Math.Sign(e.Delta) * gWheel
                If xI1 > 0 Then xI1 = 0
                If xI1 < VSR.Minimum Then xI1 = VSR.Minimum
                VSR.Value = xI1
        End Select
    End Sub

    Private Sub PMainInPaint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PMainIn.Paint, PMainInL.Paint, PMainInR.Paint
        RefreshPanel(sender.Tag, e.ClipRectangle)
    End Sub
End Class