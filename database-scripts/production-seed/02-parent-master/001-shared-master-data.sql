-- Canonical shared master data seed. Generated from workforcedb_34hi_duis on 2026-09-28.
-- Contains no tenant, employee, login, token, gateway, SMTP, or payment transaction data.
BEGIN;


-- Dumped from database version 18.6 (Debian 18.6-1.pgdg12+2)
-- Dumped by pg_dump version 18.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Data for Name: AttendanceDeviceType; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."AttendanceDeviceType" ("Id", "DeviceType", "Remark", "IsActive", "IsDeviceRegister", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "DeviceTypeCode") VALUES (1, 'Biometric Device', 'Attendance received from a registered physical attendance device.', true, true, 1, '2026-09-24 16:18:58.033773+00', NULL, NULL, 'BIOMETRIC');
INSERT INTO axionpro."AttendanceDeviceType" ("Id", "DeviceType", "Remark", "IsActive", "IsDeviceRegister", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "DeviceTypeCode") VALUES (2, 'Web', 'Attendance marked from an authenticated web client.', true, false, 1, '2026-09-24 16:18:58.033773+00', NULL, NULL, 'WEB');
INSERT INTO axionpro."AttendanceDeviceType" ("Id", "DeviceType", "Remark", "IsActive", "IsDeviceRegister", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "DeviceTypeCode") VALUES (3, 'Mobile', 'Attendance marked from an authenticated mobile client.', true, false, 1, '2026-09-24 16:18:58.033773+00', NULL, NULL, 'MOBILE');
INSERT INTO axionpro."AttendanceDeviceType" ("Id", "DeviceType", "Remark", "IsActive", "IsDeviceRegister", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "DeviceTypeCode") VALUES (4, 'Manual Entry', 'Attendance entered by an authorized administrator.', true, false, 1, '2026-09-24 16:18:58.033773+00', NULL, NULL, 'MANUAL');


--
-- Data for Name: ClientType; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (1, 'TechNova Solutions Pvt. Ltd. ddd', true, '', '');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (2, 'PVT Limited', true, 'Private Limited Company', 'Privately owned and operated business');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (3, 'Govt', true, 'Government', 'Public sector organization');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (4, 'NGO', true, 'Non-Government Organization', 'Not for profit organization');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (5, 'Partnership', true, 'Partnership Firm', 'Business operated by two or more individuals');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (6, 'Sole Proprietorship', true, 'Sole Proprietor', 'Business owned and run by one individual');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (7, 'LLP', true, 'Limited Liability Partnership', 'Partnership with limited liability for partners');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (8, 'Public Limited', true, 'Public Limited Company', 'Company whose shares are traded publicly');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (9, 'Cooperative', true, 'Cooperative Society', 'Member-owned and democratically managed business');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (10, 'Joint Venture', true, 'Joint Venture', 'Business enterprise undertaken by two or more parties');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (11, 'Franchise', true, 'Franchise Business', 'Business that licenses its operations and products');
INSERT INTO axionpro."ClientType" ("Id", "TypeName", "IsActive", "Remark", "Description") VALUES (12, 'sujeet client hai', true, '', '');


--
-- Data for Name: DataViewStructure; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."DataViewStructure" ("Id", "DisplayOn", "Discription", "Remark", "IsDisplayedAtPriority") VALUES (2, 'Top-Bar', 'Display in Top', 'Can change view accordingly', false);
INSERT INTO axionpro."DataViewStructure" ("Id", "DisplayOn", "Discription", "Remark", "IsDisplayedAtPriority") VALUES (1, 'Left-Menu', 'Display in Left', 'Can change view accordingly', false);


--
-- Data for Name: EmailTemplate; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."EmailTemplate" ("Id", "TemplateName", "TemplateCode", "Subject", "Body", "FromEmail", "FromName", "CcEmail", "BccEmail", "Category", "LanguageCode", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "AddedFromIP", "UpdatedFromIP") VALUES (2, 'Forgot Password', 'FORGOT_PASSWORD', 'Reset Your AxionPro Password', '<html>
  <body style=''font-family: Arial, sans-serif; padding: 20px;''>
    <h2 style=''color: #2E86C1;''>Dear {{UserName}},</h2>
    <p style=''font-size: 16px;''>
        This is a <strong>verification OTP</strong> sent by <em>Axion-Pro</em>.
        The OTP will expire in <strong>5 minutes</strong>.
    </p>
    <p style=''font-size: 20px; font-weight: bold; color: #e74c3c;''>OTP: {{Otp}}</p>
    <p style=''font-size: 14px; color: gray;''>
        Regards,<br/>
        <b>Axion-Pro Team</b>
    </p>
  </body>
</html>
', 'hr@quecksilber.in', 'AxionPro Support', NULL, NULL, 'System', 'en', true, 1, '2025-06-03 05:05:11.41+00', NULL, NULL, '192.168.1.100', NULL);
INSERT INTO axionpro."EmailTemplate" ("Id", "TemplateName", "TemplateCode", "Subject", "Body", "FromEmail", "FromName", "CcEmail", "BccEmail", "Category", "LanguageCode", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "AddedFromIP", "UpdatedFromIP") VALUES (4, 'Leave Approved', 'LEAVE_APPROVAL', 'Your Leave Request has been Approved', 'Hello {{UserName}},<br/><br/>Your leave request from {{LeaveStartDate}} to {{LeaveEndDate}} has been approved.<br/><br/>Regards,<br/>HR Department', 'hr@quecksilber.in', 'HR Department', NULL, NULL, 'HR', 'en', true, 1, '2025-06-03 05:05:11.41+00', NULL, NULL, '192.168.1.100', NULL);
INSERT INTO axionpro."EmailTemplate" ("Id", "TemplateName", "TemplateCode", "Subject", "Body", "FromEmail", "FromName", "CcEmail", "BccEmail", "Category", "LanguageCode", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "AddedFromIP", "UpdatedFromIP") VALUES (3, 'Birthday Wish', 'BIRTHDAY_WISH', 'Happy Birthday {{UserName}}!', 'Dear {{UserName}},<br/><br/>Wishing you a fantastic birthday and a wonderful year ahead!<br/><br/>Best wishes,<br/>HR Team', 'hr@quecksilber.in', 'HR Department', NULL, NULL, 'HR', 'en', true, 1, '2025-06-03 05:05:11.41+00', 1, '2026-09-09 09:46:04.578002+00', '192.168.1.100', NULL);
INSERT INTO axionpro."EmailTemplate" ("Id", "TemplateName", "TemplateCode", "Subject", "Body", "FromEmail", "FromName", "CcEmail", "BccEmail", "Category", "LanguageCode", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "AddedFromIP", "UpdatedFromIP") VALUES (5, 'Account Verification', 'ACCOUNT_VERIFICATION', 'Verify Your AxionProAccount', '<p></p><p style="text-align: center;"><img src="https://brandlogos.net/wp-content/uploads/2023/10/vi-vodafone-idea-logo.png" alt="" width="63" height="62"></p><p></p><p></p><p>Dear {{UserName}},<br><br>Thank you for registering with EMS.<br>Please <a target="_blank" rel="noopener" href="{{VerificationLink}}">click here</a> to verify your account.<br><br>If you did not sign up, please ignore this email.<br><br>Regards,<br>EMS Support Team</p>', 'hr@quecksilber.in', 'AxionPro Support', NULL, NULL, 'System', 'en', true, 1, '2025-06-03 05:05:11.41+00', 1, '2026-09-22 20:36:53.663557+00', '192.168.1.100', NULL);
INSERT INTO axionpro."EmailTemplate" ("Id", "TemplateName", "TemplateCode", "Subject", "Body", "FromEmail", "FromName", "CcEmail", "BccEmail", "Category", "LanguageCode", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "AddedFromIP", "UpdatedFromIP") VALUES (1, 'Welcome Email', 'WELCOME_EMAIL', 'Welcome to the AxionPro System!', '<p></p><p style="text-align: center;"><img src="http://localhost:4200/images/logo-small.svg" alt="" width="67" height="67"></p><p style="text-align: center;"></p><p style="text-align: center;"><strong>{{TenantName}}</strong></p><p>Dear <strong>{{UserName}}</strong>,</p><p><span style="color: rgb(153, 27, 27);">Welcome </span>to <strong>{{TenantName}}</strong>!</p><p></p><p>Your account has been <span style="background-color: rgb(250, 204, 21);">successfully </span>created by your organization''s <span style="color: rgb(245, 158, 11);">HR team</span>.</p><p><span style="color: rgb(21, 128, 61);">Please click the button below to set your password and activate your account.</span></p><p style="text-align: center;"><a target="_blank" rel="noopener" href="{{VerificationUrl}}"><strong>Set Your Password</strong></a></p><p></p><p>⏳ <strong>Link expiry:</strong> This link will expire in <strong>{{LinkExpiryMinutes}} minutes</strong>.</p><p>For security reasons, please do not share this link with anyone.</p><p></p><p>Regards,<br><strong>{{TenantName}} Team</strong></p><p style="text-align: center;">Need help? Contact us at <a target="_blank" rel="noopener" href="mailto:{{SupportEmail}}"><strong>{{SupportEmail}}</strong></a></p><p style="text-align: center;">© {{Year}} {{TenantName}}. All rights reserved.</p>', 'hr@quecksilber.in', 'AxionPro Notifications', 'sujeetjaiswara2012@gmail.com;mca.deepesh@gmail.com', NULL, 'System', 'en', true, 1, '2025-06-03 05:05:11.41+00', 1, '2026-09-20 06:24:49.173122+00', '192.168.1.100', NULL);


--
-- Data for Name: Gender; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."Gender" ("Id", "GenderName") VALUES (2, 'Female');
INSERT INTO axionpro."Gender" ("Id", "GenderName") VALUES (1, 'Male');
INSERT INTO axionpro."Gender" ("Id", "GenderName") VALUES (3, 'Other');


--
-- Data for Name: EmployeeType; Type: TABLE DATA; Schema: axionpro; Owner: -
-- The registration flow uses this retained global row as the template from
-- which it creates the tenant-owned Permanent employee type.
--

INSERT INTO axionpro."EmployeeType"
    ("Id", "TenantId", "TypeName", "Description", "Remark", "IsActive",
     "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime",
     "IsSoftDeleted", "SoftDeletedById", "SoftDeletedDateTime")
VALUES
    (1, NULL, 'Permanent', 'Permanent employee type used as the tenant onboarding template',
     'Canonical global onboarding template', true, 1, CURRENT_TIMESTAMP,
     NULL, NULL, false, NULL, NULL),
    (2, NULL, 'Contract', 'Employee engaged for a fixed contractual term',
     'Canonical global employment type', true, 1, CURRENT_TIMESTAMP,
     NULL, NULL, false, NULL, NULL),
    (3, NULL, 'Intern', 'Student or trainee engaged for a defined internship period',
     'Canonical global employment type', true, 1, CURRENT_TIMESTAMP,
     NULL, NULL, false, NULL, NULL),
    (4, NULL, 'Part-Time', 'Employee engaged for reduced or flexible working hours',
     'Canonical global employment type', true, 1, CURRENT_TIMESTAMP,
     NULL, NULL, false, NULL, NULL),
    (5, NULL, 'Freelancer', 'Independent professional engaged for project-based work',
     'Canonical global employment type', true, 1, CURRENT_TIMESTAMP,
     NULL, NULL, false, NULL, NULL),
    (6, NULL, 'Probationer', 'New employee serving a probation period before confirmation',
     'Canonical global pre-confirmation employment type', true, 1, CURRENT_TIMESTAMP,
     NULL, NULL, false, NULL, NULL);


--
-- Data for Name: SubscriptionPlan; Type: TABLE DATA; Schema: axionpro; Owner: -
-- Exactly three clean host-managed plans are retained after the canonical reset.
--

INSERT INTO axionpro."SubscriptionPlan"
    ("Id", "PlanName", "MaxUsers", "PerDayPrice", "MonthlyPrice",
     "YearlyPrice", "IsFree", "IsActive", "AddedDateTime", "AddedById",
     "UpdatedById", "UpdatedDateTime", "CurrencyKey", "IsMostPopular",
     "IsCustom", "IsSoftDeleted", "DeletedById", "DeletedDateTime")
VALUES
    (1, 'Starter', 25, 0.00, 499.00, 4999.00, false, true,
     CURRENT_TIMESTAMP, 1, NULL, NULL, 'INR', false, false, false, NULL, NULL),
    (2, 'Professional', 100, 0.00, 1499.00, 14999.00, false, true,
     CURRENT_TIMESTAMP, 1, NULL, NULL, 'INR', true, false, false, NULL, NULL),
    (3, 'Enterprise', 500, 0.00, 4999.00, 49999.00, false, true,
     CURRENT_TIMESTAMP, 1, NULL, NULL, 'INR', false, true, false, NULL, NULL);


--
-- Data for Name: IdentityCategory; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."IdentityCategory" ("Id", "Code", "Name", "Description", "IsActive", "AddedById", "UpdatedById", "AddedDateTime", "UpdatedDateTime") VALUES (1, 'TAX', 'Tax Identification', 'Documents used for tax purposes like PAN, ITIN, SSN', true, NULL, NULL, '2025-12-22 18:04:12.523+00', NULL);
INSERT INTO axionpro."IdentityCategory" ("Id", "Code", "Name", "Description", "IsActive", "AddedById", "UpdatedById", "AddedDateTime", "UpdatedDateTime") VALUES (3, 'SOCIALSECURITY', 'Social Security', 'Social security or national insurance related documents like SSN, SIN', true, NULL, NULL, '2025-12-22 18:04:12.523+00', NULL);
INSERT INTO axionpro."IdentityCategory" ("Id", "Code", "Name", "Description", "IsActive", "AddedById", "UpdatedById", "AddedDateTime", "UpdatedDateTime") VALUES (4, 'VOTER', 'Voter Identity', 'Voter identification documents like EPIC in India', true, NULL, NULL, '2025-12-22 18:04:12.523+00', NULL);


--
-- Data for Name: IdentityCategoryDocument; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."IdentityCategoryDocument" ("Id", "IdentityCategoryId", "Code", "DocumentName", "Description", "IsUnique", "IsActive", "AddedById", "UpdatedById", "AddedDateTime", "UpdatedDateTime") VALUES (3, 4, 'EPIC', 'Voter ID', 'Indian voter identification', true, true, NULL, NULL, '2025-12-22 18:08:02.173+00', NULL);
INSERT INTO axionpro."IdentityCategoryDocument" ("Id", "IdentityCategoryId", "Code", "DocumentName", "Description", "IsUnique", "IsActive", "AddedById", "UpdatedById", "AddedDateTime", "UpdatedDateTime") VALUES (5, 1, 'ITIN', 'Individual Taxpayer Identification Number', 'US tax ID for non-residents', true, true, NULL, NULL, '2025-12-22 18:08:02.173+00', NULL);
INSERT INTO axionpro."IdentityCategoryDocument" ("Id", "IdentityCategoryId", "Code", "DocumentName", "Description", "IsUnique", "IsActive", "AddedById", "UpdatedById", "AddedDateTime", "UpdatedDateTime") VALUES (7, 3, 'SIN', 'Social Insurance Number', 'Canada social insurance number', true, true, NULL, NULL, '2025-12-22 18:08:02.177+00', NULL);
INSERT INTO axionpro."IdentityCategoryDocument" ("Id", "IdentityCategoryId", "Code", "DocumentName", "Description", "IsUnique", "IsActive", "AddedById", "UpdatedById", "AddedDateTime", "UpdatedDateTime") VALUES (8, 3, 'UAN', 'Universal Account Number', 'India EPF Universal Account Number', true, true, 317, NULL, '2025-12-23 19:19:34.983+00', NULL);


--
-- Data for Name: LocalityType; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."LocalityType" ("Id", "TypeName", "IsActive") VALUES (1, 'City', true);
INSERT INTO axionpro."LocalityType" ("Id", "TypeName", "IsActive") VALUES (2, 'Town', true);
INSERT INTO axionpro."LocalityType" ("Id", "TypeName", "IsActive") VALUES (3, 'Village', true);
INSERT INTO axionpro."LocalityType" ("Id", "TypeName", "IsActive") VALUES (4, 'Other / Unclassified', true);


--
-- Data for Name: PageTypeEnum; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."PageTypeEnum" ("Id", "PageTypeName") VALUES (1, 'Master');
INSERT INTO axionpro."PageTypeEnum" ("Id", "PageTypeName") VALUES (3, 'View');
INSERT INTO axionpro."PageTypeEnum" ("Id", "PageTypeName") VALUES (4, 'Common');
INSERT INTO axionpro."PageTypeEnum" ("Id", "PageTypeName") VALUES (5, 'Report');
INSERT INTO axionpro."PageTypeEnum" ("Id", "PageTypeName") VALUES (2, 'Transaction');


--
-- Data for Name: PolicyCategory; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (1, 'LEAVE', 'Leave', 'Leave eligibility, accrual, carry-forward and sandwich rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (2, 'ATTENDANCE', 'Attendance', 'Attendance channel, late, overtime and regularization rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (3, 'WORK_ARRANGEMENT', 'Work Arrangement', 'Office, remote, hybrid and field-work rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (4, 'TRAVEL', 'Travel', 'Travel eligibility, limits and reimbursement rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (5, 'ACCOMMODATION', 'Accommodation', 'Accommodation eligibility and allowance rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (6, 'INSURANCE', 'Insurance', 'Insurance eligibility, dependent and coverage rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (7, 'EXPENSE', 'Expense and Reimbursement', 'Expense categories, limits and evidence rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (8, 'HOLIDAY', 'Holiday and Calendar', 'Location-specific holiday and optional holiday rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (9, 'SHIFT', 'Shift and Weekly Off', 'Shift, weekly-off and roster rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (10, 'EMPLOYMENT', 'Employment Lifecycle', 'Probation, confirmation and notice-period rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (11, 'BENEFIT', 'Employee Benefit', 'Tenant-defined benefits and allowance rules.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);
INSERT INTO axionpro."PolicyCategory" ("Id", "CategoryCode", "CategoryName", "Description", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime") VALUES (12, 'CUSTOM', 'Custom', 'Tenant-defined policy category.', true, NULL, '2026-09-16 07:28:35.410156+00', NULL, NULL);


--
-- Data for Name: PolicyDocumentType; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."PolicyDocumentType" ("Id", "DocumentTypeCode", "DocumentTypeName", "IsActive") VALUES (1, 'POLICY_DOCUMENT', 'Policy Document', true);
INSERT INTO axionpro."PolicyDocumentType" ("Id", "DocumentTypeCode", "DocumentTypeName", "IsActive") VALUES (2, 'ANNEXURE', 'Annexure', true);
INSERT INTO axionpro."PolicyDocumentType" ("Id", "DocumentTypeCode", "DocumentTypeName", "IsActive") VALUES (3, 'LEGAL_CIRCULAR', 'Legal Circular', true);
INSERT INTO axionpro."PolicyDocumentType" ("Id", "DocumentTypeCode", "DocumentTypeName", "IsActive") VALUES (4, 'EMPLOYEE_GUIDE', 'Employee Guide', true);
INSERT INTO axionpro."PolicyDocumentType" ("Id", "DocumentTypeCode", "DocumentTypeName", "IsActive") VALUES (5, 'TRANSLATION', 'Translated Policy', true);


--
-- Data for Name: PolicyRuleType; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (1, 'ELIGIBILITY', 'Eligibility', 'Determines who qualifies for the policy.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (2, 'ENTITLEMENT', 'Entitlement', 'Defines entitlement quantity and unit.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (3, 'ACCRUAL', 'Accrual', 'Defines accrual frequency and calculation.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (4, 'CARRY_FORWARD', 'Carry Forward', 'Defines year-end carry-forward behavior.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (5, 'SANDWICH', 'Sandwich', 'Defines holiday and weekly-off sandwich behavior.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (6, 'LIMIT', 'Limit', 'Defines monetary, count or duration limits.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (7, 'ATTENDANCE_CHANNEL', 'Attendance Channel', 'Defines allowed web, mobile, biometric or manual channels.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (8, 'LATE_PENALTY', 'Late Penalty', 'Defines late grace and penalty thresholds.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (9, 'OVERTIME', 'Overtime', 'Defines overtime eligibility and calculation.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (10, 'APPROVAL', 'Approval', 'Defines request approval requirements.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (11, 'REIMBURSEMENT', 'Reimbursement', 'Defines reimbursement evidence and amount rules.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (12, 'CALENDAR', 'Calendar', 'Defines holiday/calendar application behavior.', 1, true);
INSERT INTO axionpro."PolicyRuleType" ("Id", "RuleTypeCode", "RuleTypeName", "Description", "JsonSchemaVersion", "IsActive") VALUES (13, 'CUSTOM', 'Custom Rule', 'Tenant-defined structured JSON rule.', 1, true);


--
-- Data for Name: PolicyStatus; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."PolicyStatus" ("Id", "StatusCode", "StatusName", "IsTerminal", "IsActive") VALUES (1, 'DRAFT', 'Draft', false, true);
INSERT INTO axionpro."PolicyStatus" ("Id", "StatusCode", "StatusName", "IsTerminal", "IsActive") VALUES (2, 'UNDER_REVIEW', 'Under Review', false, true);
INSERT INTO axionpro."PolicyStatus" ("Id", "StatusCode", "StatusName", "IsTerminal", "IsActive") VALUES (3, 'APPROVED', 'Approved', false, true);
INSERT INTO axionpro."PolicyStatus" ("Id", "StatusCode", "StatusName", "IsTerminal", "IsActive") VALUES (4, 'PUBLISHED', 'Published', false, true);
INSERT INTO axionpro."PolicyStatus" ("Id", "StatusCode", "StatusName", "IsTerminal", "IsActive") VALUES (5, 'SUSPENDED', 'Suspended', false, true);
INSERT INTO axionpro."PolicyStatus" ("Id", "StatusCode", "StatusName", "IsTerminal", "IsActive") VALUES (6, 'ARCHIVED', 'Archived', true, true);
INSERT INTO axionpro."PolicyStatus" ("Id", "StatusCode", "StatusName", "IsTerminal", "IsActive") VALUES (7, 'REJECTED', 'Rejected', true, true);


--
-- Data for Name: TenantIndustry; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."TenantIndustry" ("Id", "IndustryName", "Description", "Remark", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "IsSoftDeted", "SoftDeletedById", "SoftDeletedDateTime") VALUES (9, 'Information Technology', 'IT services and consulting', 'Dummy IT industry', true, 1, '2025-09-26 12:16:55.99+00', NULL, NULL, false, NULL, NULL);
INSERT INTO axionpro."TenantIndustry" ("Id", "IndustryName", "Description", "Remark", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "IsSoftDeted", "SoftDeletedById", "SoftDeletedDateTime") VALUES (10, 'Manufacturing', 'Production and assembly industry', 'Dummy manufacturing', true, 1, '2025-09-26 12:16:55.99+00', NULL, NULL, false, NULL, NULL);
INSERT INTO axionpro."TenantIndustry" ("Id", "IndustryName", "Description", "Remark", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "IsSoftDeted", "SoftDeletedById", "SoftDeletedDateTime") VALUES (11, 'Healthcare', 'Hospitals and clinics', 'Dummy healthcare', true, 1, '2025-09-26 12:16:55.99+00', NULL, NULL, false, NULL, NULL);
INSERT INTO axionpro."TenantIndustry" ("Id", "IndustryName", "Description", "Remark", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "IsSoftDeted", "SoftDeletedById", "SoftDeletedDateTime") VALUES (12, 'Education', 'Schools, colleges, and universities', 'Dummy education', true, 1, '2025-09-26 12:16:55.99+00', NULL, NULL, false, NULL, NULL);
INSERT INTO axionpro."TenantIndustry" ("Id", "IndustryName", "Description", "Remark", "IsActive", "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime", "IsSoftDeted", "SoftDeletedById", "SoftDeletedDateTime") VALUES (13, 'Finance', 'Banking, insurance, and investments', 'Dummy finance industry', true, 1, '2025-09-26 12:16:55.99+00', NULL, NULL, false, NULL, NULL);


--
-- Data for Name: TenderStatus; Type: TABLE DATA; Schema: axionpro; Owner: -
--

INSERT INTO axionpro."TenderStatus" ("Id", "StatusName", "Description", "Remark", "IsActive") VALUES (1, 'Open', 'Tender is open for bidding', 'Available for submission', true);
INSERT INTO axionpro."TenderStatus" ("Id", "StatusName", "Description", "Remark", "IsActive") VALUES (2, 'Closed', 'Tender is closed for submissions', 'No further bids accepted', true);
INSERT INTO axionpro."TenderStatus" ("Id", "StatusName", "Description", "Remark", "IsActive") VALUES (3, 'Pending', 'Tender is pending review', 'Awaiting evaluation', true);
INSERT INTO axionpro."TenderStatus" ("Id", "StatusName", "Description", "Remark", "IsActive") VALUES (4, 'Awarded', 'Tender has been awarded', 'Contract finalized', true);
INSERT INTO axionpro."TenderStatus" ("Id", "StatusName", "Description", "Remark", "IsActive") VALUES (5, 'Cancelled', 'Tender has been cancelled', 'No further process', true);


--
-- Name: AttendanceDeviceType_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."AttendanceDeviceType_Id_seq"', 4, true);


--
-- Name: ClientType_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."ClientType_Id_seq"', 1, false);


--
-- Name: ComplianceTypeMaster_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."ComplianceTypeMaster_Id_seq"', 33, true);


--
-- Name: DataViewStructure_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."DataViewStructure_Id_seq"', 1, false);


--
-- Name: EmailTemplate_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."EmailTemplate_Id_seq"', 1, false);


--
-- Name: Gender_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."Gender_Id_seq"', 1, false);


--
-- Name: IdentityCategoryDocument_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."IdentityCategoryDocument_Id_seq"', 1, true);


--
-- Name: IdentityCategory_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."IdentityCategory_Id_seq"', 1, false);


--
-- Name: LocalityType_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."LocalityType_Id_seq"', 1, false);


--
-- Name: PolicyCategory_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."PolicyCategory_Id_seq"', 48, true);


--
-- Name: PolicyRuleType_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."PolicyRuleType_Id_seq"', 52, true);


--
-- Name: TenantIndustry_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."TenantIndustry_Id_seq"', 1, false);


--
-- Name: TenderStatus_Id_seq; Type: SEQUENCE SET; Schema: axionpro; Owner: -
--

SELECT pg_catalog.setval('axionpro."TenderStatus_Id_seq"', 1, false);


-- Synchronize every explicitly seeded identity sequence with its actual table
-- maximum. The source database contained several stale sequence values, so the
-- dump-provided setval statements above cannot safely determine the next Id.
DO
$$
DECLARE
    seeded_table_name text;
    seeded_sequence_name text;
    maximum_id bigint;
BEGIN
    FOREACH seeded_table_name IN ARRAY ARRAY[
        'AttendanceDeviceType',
        'ClientType',
        'ComplianceTypeMaster',
        'DataViewStructure',
        'EmailTemplate',
        'EmployeeType',
        'Gender',
        'IdentityCategoryDocument',
        'IdentityCategory',
        'LocalityType',
        'PolicyCategory',
        'PolicyRuleType',
        'SubscriptionPlan',
        'TenantIndustry',
        'TenderStatus'
    ]
    LOOP
        seeded_sequence_name := pg_get_serial_sequence(
            format('axionpro.%I', seeded_table_name),
            'Id'
        );

        IF seeded_sequence_name IS NOT NULL THEN
            EXECUTE format(
                'SELECT COALESCE(MAX("Id"), 0) FROM axionpro.%I',
                seeded_table_name
            ) INTO maximum_id;

            IF maximum_id = 0 THEN
                PERFORM setval(seeded_sequence_name::regclass, 1, FALSE);
            ELSE
                PERFORM setval(seeded_sequence_name::regclass, maximum_id, TRUE);
            END IF;
        END IF;
    END LOOP;
END;
$$;


--
-- PostgreSQL database dump complete
--

COMMIT;
