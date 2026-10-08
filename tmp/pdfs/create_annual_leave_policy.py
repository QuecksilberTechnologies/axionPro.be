from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.units import mm
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak
from pathlib import Path

out = Path(r'C:\AxionProCodeBase\QuecksilberTechnologies\output\pdf\AnnualLeave2026-2027.pdf')
out.parent.mkdir(parents=True, exist_ok=True)
styles = getSampleStyleSheet()
styles.add(ParagraphStyle(name='TitleCenter', parent=styles['Title'], alignment=TA_CENTER, textColor=colors.HexColor('#17365D'), spaceAfter=8))
styles.add(ParagraphStyle(name='Subtle', parent=styles['Normal'], alignment=TA_CENTER, textColor=colors.HexColor('#5B6573'), fontSize=9, leading=12))
styles.add(ParagraphStyle(name='H1Blue', parent=styles['Heading1'], textColor=colors.HexColor('#17365D'), fontSize=15, leading=19, spaceBefore=10, spaceAfter=6))
styles.add(ParagraphStyle(name='Body2', parent=styles['BodyText'], fontSize=9.5, leading=14, spaceAfter=6))
styles.add(ParagraphStyle(name='Small', parent=styles['BodyText'], fontSize=8.5, leading=12))

def footer(canvas, doc):
    canvas.saveState()
    canvas.setStrokeColor(colors.HexColor('#D9E2F3'))
    canvas.line(18*mm, 15*mm, 192*mm, 15*mm)
    canvas.setFont('Helvetica', 8)
    canvas.setFillColor(colors.HexColor('#65758B'))
    canvas.drawString(18*mm, 10*mm, 'TechNova Solutions Pvt. Ltd. - Draft Policy Document')
    canvas.drawRightString(192*mm, 10*mm, f'Page {doc.page}')
    canvas.restoreState()

doc = SimpleDocTemplate(str(out), pagesize=A4, rightMargin=18*mm, leftMargin=18*mm, topMargin=17*mm, bottomMargin=20*mm, title='AnnualLeave2026-2027')
story = [
    Paragraph('Annual Leave Policy 2026-2027', styles['TitleCenter']),
    Paragraph('Policy code: ANNUAL_LEAVE_2026_2027 | Status: Draft | Effective period: 01 Apr 2026 to 31 Mar 2027', styles['Subtle']),
    Spacer(1, 6*mm),
    Paragraph('1. Purpose', styles['H1Blue']),
    Paragraph('This draft defines annual leave components, eligibility, accrual, lapse and carry-forward rules for eligible employees. Each leave component is administered independently so that one component does not inherit another component\'s balance or eligibility behavior.', styles['Body2']),
    Paragraph('2. Leave components', styles['H1Blue'])
]
rows = [
    ['Leave type', 'Entitlement / accrual', 'Carry forward', 'Primary eligibility'],
    ['Casual Leave', '6 days per policy year; annual grant', 'No; unused balance lapses at year end', 'Eligible active employees'],
    ['Earned Leave', '1.5 days per completed month', 'Yes; maximum 30 days', 'Eligible active employees after confirmation'],
    ['Sick Leave', '6 days per policy year', 'No; unused balance lapses at year end', 'Eligible active employees; evidence may be required'],
    ['Maternity Leave', '182 calendar days, subject to applicable law', 'Not applicable', 'Eligible female employees'],
    ['Paternity Leave', '15 calendar days per eligible event', 'Not applicable', 'Eligible male employees'],
    ['Menstrual Leave', '1 day per month; maximum 12 days per policy year', 'No; unused balance lapses monthly/year end', 'Eligible female employees']
]
rows = [[Paragraph(str(cell), styles['Small']) for cell in row] for row in rows]
t = Table(rows, colWidths=[30*mm, 48*mm, 48*mm, 48*mm], repeatRows=1)
t.setStyle(TableStyle([
    ('BACKGROUND',(0,0),(-1,0),colors.HexColor('#17365D')), ('TEXTCOLOR',(0,0),(-1,0),colors.white),
    ('FONTNAME',(0,0),(-1,0),'Helvetica-Bold'), ('FONTSIZE',(0,0),(-1,-1),7.5),
    ('LEADING',(0,0),(-1,-1),10.5), ('VALIGN',(0,0),(-1,-1),'TOP'),
    ('GRID',(0,0),(-1,-1),0.4,colors.HexColor('#B4C6E7')), ('ROWBACKGROUNDS',(0,1),(-1,-1),[colors.white,colors.HexColor('#F5F8FC')]),
    ('LEFTPADDING',(0,0),(-1,-1),5), ('RIGHTPADDING',(0,0),(-1,-1),5), ('TOPPADDING',(0,0),(-1,-1),5), ('BOTTOMPADDING',(0,0),(-1,-1),5)
]))
story += [t,
    Paragraph('3. Rule isolation', styles['H1Blue']),
    Paragraph('Rules are linked to one or more selected leave types. For example, the Earned Leave carry-forward rule applies only to Earned Leave. Casual, Sick and Menstrual Leave balances do not carry forward. Gender eligibility is maintained as an applicability condition for the corresponding leave type and does not affect unrelated leave components.', styles['Body2']),
    Paragraph('4. General conditions', styles['H1Blue']),
    Paragraph('Leave requests remain subject to available balance, required evidence, manager approval, statutory requirements and operational needs. Where applicable law provides a more beneficial entitlement, the legal entitlement will prevail. Unauthorized absence and false evidence may be handled under the applicable conduct process.', styles['Body2']),
    Paragraph('5. Administration and version control', styles['H1Blue']),
    Paragraph('This document is attached to a Draft policy version. The approved and published version, its rules, applicability conditions and checksum become immutable. Any later amendment must be made through a new Draft version and the normal review and publication workflow.', styles['Body2']),
    Spacer(1, 7*mm),
    Table([
        ['Prepared for', 'TechNova Solutions Pvt. Ltd.'],
        ['Document language', 'English'],
        ['Employee visibility', 'Visible to employees'],
        ['Review status', 'Draft - pending review and approval']
    ], colWidths=[45*mm, 125*mm], style=TableStyle([
        ('BACKGROUND',(0,0),(0,-1),colors.HexColor('#EAF0F8')), ('FONTNAME',(0,0),(0,-1),'Helvetica-Bold'),
        ('GRID',(0,0),(-1,-1),0.4,colors.HexColor('#B4C6E7')), ('FONTSIZE',(0,0),(-1,-1),9),
        ('VALIGN',(0,0),(-1,-1),'TOP'), ('LEFTPADDING',(0,0),(-1,-1),6), ('RIGHTPADDING',(0,0),(-1,-1),6),
        ('TOPPADDING',(0,0),(-1,-1),6), ('BOTTOMPADDING',(0,0),(-1,-1),6)
    ]))
]
doc.build(story, onFirstPage=footer, onLaterPages=footer)
print(out)

