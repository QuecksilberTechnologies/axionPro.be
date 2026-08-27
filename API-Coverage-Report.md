# Angular–Backend API Coverage Report

Generated: 2026-08-28 02:21:45 +05:30

## Scope and matching rules

- Angular source: `C:\latestAxionProUI\axionpro-app\src` (runtime `*.ts` only; `*.spec.ts` and commented-out code excluded).
- Backend source: `C:\AxionProCodeBase\QuecksilberTechnologies\axionpro.api\Controllers` (active controller actions only; commented-out code excluded).
- A frontend API is considered matched only when HTTP verb and normalized route both match. Matching ignores case, the `api/` prefix, query strings, and route-parameter names/types (for example, `{id:int}` equals `{id}`).
- `data/*.json` calls are counted separately as local/mock HTTP assets; they are not backend API candidates.

## Executive summary

| Measure | Total |
|---|---:|
| Angular runtime TypeScript files scanned | 716 |
| Angular HTTP calls found (all active calls) | 314 |
| Angular backend API HTTP call-sites | 270 |
| Angular backend API endpoint paths found (includes conditional branches) | 272 |
| Angular unique backend API endpoints | 265 |
| Local/mock HTTP call-sites | 43 |
| Unresolved/external HTTP call-sites | 1 |
| Backend controller files scanned | 69 |
| Backend controller files with active HTTP actions | 68 |
| Backend unique API endpoints | 281 |
| Exact Angular ↔ backend endpoint matches | 206 |
| Angular endpoint method mismatches | 3 |
| Angular endpoints missing in backend | 56 |
| Backend endpoints with no exact Angular call | 75 |
| Angular endpoint exact-match coverage | 77.7% (206/265) |
| Backend endpoint Angular-consumption coverage | 73.3% (206/281) |

## Discrepancies requiring review

| Status | Angular request | Backend evidence | Angular source |
|---|---|---|---|
| Method mismatch | GET /api/PlanModuleMapping/save | Backend verb(s): POST | app/core/services/plan-module-mapping-api.ts:24 |
| Method mismatch | GET /api/Subscription/{} | Backend verb(s): PUT | app/core/services/subscriptions-api.ts:36<br>app/core/services/subscriptions-api.ts:58<br>app/core/services/subscriptions-api.ts:70 |
| Method mismatch | POST /api/Subscription/{} | Backend verb(s): PUT | app/core/services/subscriptions-api.ts:49<br>app/core/services/subscriptions-api.ts:79<br>app/core/services/subscriptions-api.ts:86 |
| Route missing in backend | POST /api/Asset/assign-employee |  | app/core/services/assets-api.ts:54 |
| Route missing in backend | POST /api/AttendancePolicy/create |  | app/core/services/attendance-policy-api.ts:40 |
| Route missing in backend | DELETE /api/AttendancePolicy/delete/{} |  | app/core/services/attendance-policy-api.ts:62 |
| Route missing in backend | GET /api/AttendancePolicy/get-all |  | app/core/services/attendance-policy-api.ts:28 |
| Route missing in backend | GET /api/AttendancePolicy/get-by-id/{} |  | app/core/services/attendance-policy-api.ts:34 |
| Route missing in backend | POST /api/AttendancePolicy/update |  | app/core/services/attendance-policy-api.ts:47 |
| Route missing in backend | POST /api/AttendancePolicy/update-status |  | app/core/services/attendance-policy-api.ts:56 |
| Route missing in backend | POST /api/Auth/reset-login-new-password |  | app/core/services/auth-api.ts:187 |
| Route missing in backend | POST /api/Employee/assign-role |  | app/core/services/employee-api.ts:161 |
| Route missing in backend | POST /api/Employee/Sensitive/update |  | app/core/services/employee-identity-api.ts:78 |
| Route missing in backend | POST /api/EmployeeDeviceEnrollment/create |  | app/core/services/employee-device-enrollment-api.ts:50 |
| Route missing in backend | DELETE /api/EmployeeDeviceEnrollment/delete/{} |  | app/core/services/employee-device-enrollment-api.ts:81 |
| Route missing in backend | GET /api/EmployeeDeviceEnrollment/get-all |  | app/core/services/employee-device-enrollment-api.ts:30 |
| Route missing in backend | GET /api/EmployeeDeviceEnrollment/get-by-id/{} |  | app/core/services/employee-device-enrollment-api.ts:40 |
| Route missing in backend | POST /api/EmployeeDeviceEnrollment/update |  | app/core/services/employee-device-enrollment-api.ts:61 |
| Route missing in backend | POST /api/EmployeeDeviceEnrollment/update-status |  | app/core/services/employee-device-enrollment-api.ts:71 |
| Route missing in backend | POST /api/EmployeeLocationAssignment/create |  | app/core/services/employee-location-assignment-api.ts:52 |
| Route missing in backend | DELETE /api/EmployeeLocationAssignment/delete/{} |  | app/core/services/employee-location-assignment-api.ts:85 |
| Route missing in backend | GET /api/EmployeeLocationAssignment/get-all |  | app/core/services/employee-location-assignment-api.ts:30 |
| Route missing in backend | GET /api/EmployeeLocationAssignment/get-by-id/{} |  | app/core/services/employee-location-assignment-api.ts:40 |
| Route missing in backend | POST /api/EmployeeLocationAssignment/update |  | app/core/services/employee-location-assignment-api.ts:63 |
| Route missing in backend | POST /api/EmployeeLocationAssignment/update-status |  | app/core/services/employee-location-assignment-api.ts:75 |
| Route missing in backend | POST /api/EmployeeWorkArrangement/create |  | app/core/services/employee-work-arrangement-api.ts:48 |
| Route missing in backend | DELETE /api/EmployeeWorkArrangement/delete/{} |  | app/core/services/employee-work-arrangement-api.ts:80 |
| Route missing in backend | GET /api/EmployeeWorkArrangement/get-all |  | app/core/services/employee-work-arrangement-api.ts:30 |
| Route missing in backend | GET /api/EmployeeWorkArrangement/get-by-id/{} |  | app/core/services/employee-work-arrangement-api.ts:38 |
| Route missing in backend | POST /api/EmployeeWorkArrangement/update |  | app/core/services/employee-work-arrangement-api.ts:59 |
| Route missing in backend | POST /api/EmployeeWorkArrangement/update-status |  | app/core/services/employee-work-arrangement-api.ts:70 |
| Route missing in backend | POST /api/EmployeeWorkModeOverride/create |  | app/core/services/employee-work-mode-override-api.ts:50 |
| Route missing in backend | DELETE /api/EmployeeWorkModeOverride/delete/{} |  | app/core/services/employee-work-mode-override-api.ts:81 |
| Route missing in backend | GET /api/EmployeeWorkModeOverride/get-all |  | app/core/services/employee-work-mode-override-api.ts:30 |
| Route missing in backend | GET /api/EmployeeWorkModeOverride/get-by-id/{} |  | app/core/services/employee-work-mode-override-api.ts:40 |
| Route missing in backend | POST /api/EmployeeWorkModeOverride/update |  | app/core/services/employee-work-mode-override-api.ts:61 |
| Route missing in backend | POST /api/EmployeeWorkModeOverride/update-status |  | app/core/services/employee-work-mode-override-api.ts:71 |
| Route missing in backend | POST /api/EmployeeWorkPattern/create |  | app/core/services/employee-work-pattern-api.ts:42 |
| Route missing in backend | DELETE /api/EmployeeWorkPattern/delete/{} |  | app/core/services/employee-work-pattern-api.ts:67 |
| Route missing in backend | GET /api/EmployeeWorkPattern/get-all |  | app/core/services/employee-work-pattern-api.ts:28 |
| Route missing in backend | GET /api/EmployeeWorkPattern/get-by-id/{} |  | app/core/services/employee-work-pattern-api.ts:34 |
| Route missing in backend | POST /api/EmployeeWorkPattern/update |  | app/core/services/employee-work-pattern-api.ts:51 |
| Route missing in backend | POST /api/EmployeeWorkPattern/update-status |  | app/core/services/employee-work-pattern-api.ts:59 |
| Route missing in backend | GET /api/Gender/Gender/get |  | app/core/services/users-api.ts:37 |
| Route missing in backend | POST /api/Leave/add-leave-type |  | app/core/services/leave-types.ts:44 |
| Route missing in backend | POST /api/Leave/delete-leave-type |  | app/core/services/leave-types.ts:56 |
| Route missing in backend | GET /api/Leave/get-all-leave-type |  | app/core/services/leave-types.ts:38 |
| Route missing in backend | POST /api/Leave/update-leave-type |  | app/core/services/leave-types.ts:50 |
| Route missing in backend | GET /api/PolicyMappingLeaveType/get-by-employee-type-Id |  | app/core/services/policy-mapping-leave-type-api.ts:67 |
| Route missing in backend | GET /api/Role/get-permissions |  | app/core/services/employee-api.ts:153<br>app/core/services/roles-api.ts:135 |
| Route missing in backend | POST /api/TenantLocation/create |  | app/core/services/tenant-location-api.ts:40 |
| Route missing in backend | DELETE /api/TenantLocation/delete/{} |  | app/core/services/tenant-location-api.ts:62 |
| Route missing in backend | GET /api/TenantLocation/get-all |  | app/core/services/tenant-location-api.ts:28 |
| Route missing in backend | GET /api/TenantLocation/get-by-id/{} |  | app/core/services/tenant-location-api.ts:34 |
| Route missing in backend | POST /api/TenantLocation/update |  | app/core/services/tenant-location-api.ts:47 |
| Route missing in backend | POST /api/TenantLocation/update-status |  | app/core/services/tenant-location-api.ts:56 |
| Route missing in backend | GET /api/Ticket/TicketHeader/get-all |  | app/features/tickets/ticket-api.ts:79 (conditional URL branch) |
| Route missing in backend | POST /api/TicketCreation/open |  | app/features/tickets/ticket-api.ts:139 |
| Route missing in backend | POST /api/UserModuleRolePermission/assign-employee-permissions |  | app/core/services/employee-api.ts:178 |

## Angular API inventory

| Status | Request | Call-sites | Source(s) | Backend mapping |
|---|---|---:|---|---|
| Matched | POST /api/Asset/add | 1 | app/core/services/assets-api.ts:28 | AssetController.AddAsset |
| Route missing in backend | POST /api/Asset/assign-employee | 1 | app/core/services/assets-api.ts:54 |  |
| Matched | POST /api/Asset/Category/add | 1 | app/core/services/asset-categories-api.ts:29 | CategoryController.AddAssetCategory |
| Matched | DELETE /api/Asset/Category/delete | 1 | app/core/services/asset-categories-api.ts:50 | CategoryController.DeleteAssetCategory |
| Matched | GET /api/Asset/Category/get | 1 | app/core/services/asset-categories-api.ts:36 | CategoryController.GetAllAssetCategory |
| Matched | PUT /api/Asset/Category/update | 1 | app/core/services/asset-categories-api.ts:43 | CategoryController.UpdateAssetCategory |
| Matched | DELETE /api/Asset/delete | 1 | app/core/services/assets-api.ts:48 | AssetController.DeleteAsset |
| Matched | GET /api/Asset/get | 1 | app/core/services/assets-api.ts:35 | AssetController.GetAllAssets |
| Matched | POST /api/Asset/Status/add | 1 | app/core/services/asset-status-api.ts:27 | StatusController.AddAssetStatus |
| Matched | DELETE /api/Asset/Status/delete | 1 | app/core/services/asset-status-api.ts:47 | StatusController.DeleteAssetStatus |
| Matched | GET /api/Asset/Status/get | 1 | app/core/services/asset-status-api.ts:34 | StatusController.GetByIdAssetStatus |
| Matched | PUT /api/Asset/Status/update | 1 | app/core/services/asset-status-api.ts:40 | StatusController.UpdateAssetStatus |
| Matched | POST /api/Asset/Type/add | 1 | app/core/services/asset-types-api.ts:31 | TypeController.AddAssetType |
| Matched | DELETE /api/Asset/Type/delete | 1 | app/core/services/asset-types-api.ts:51 | TypeController.DeleteAssetType |
| Matched | GET /api/Asset/Type/get | 1 | app/core/services/asset-types-api.ts:38 | TypeController.GetAllAssetType |
| Matched | PUT /api/Asset/Type/update | 1 | app/core/services/asset-types-api.ts:44 | TypeController.UpdateAssetType |
| Matched | PUT /api/Asset/update | 1 | app/core/services/assets-api.ts:41 | AssetController.UpdateAsset |
| Route missing in backend | POST /api/AttendancePolicy/create | 1 | app/core/services/attendance-policy-api.ts:40 |  |
| Route missing in backend | DELETE /api/AttendancePolicy/delete/{} | 1 | app/core/services/attendance-policy-api.ts:62 |  |
| Route missing in backend | GET /api/AttendancePolicy/get-all | 1 | app/core/services/attendance-policy-api.ts:28 |  |
| Route missing in backend | GET /api/AttendancePolicy/get-by-id/{} | 1 | app/core/services/attendance-policy-api.ts:34 |  |
| Route missing in backend | POST /api/AttendancePolicy/update | 1 | app/core/services/attendance-policy-api.ts:47 |  |
| Route missing in backend | POST /api/AttendancePolicy/update-status | 1 | app/core/services/attendance-policy-api.ts:56 |  |
| Matched | POST /api/Auth/create-new-password | 1 | app/core/services/auth-api.ts:199 | AuthController.CreateLoginPassword |
| Matched | POST /api/Auth/forgot-password | 1 | app/core/services/auth-api.ts:175 | AuthController.EnterLoginId |
| Matched | POST /api/Auth/refresh-token | 1 | app/core/services/auth-api.ts:169 | AuthController.RefreshToken |
| Matched | POST /api/Auth/resend-credential | 1 | app/core/services/auth-api.ts:205 | AuthController.CreateNewLoginPasswordURL |
| Route missing in backend | POST /api/Auth/reset-login-new-password | 1 | app/core/services/auth-api.ts:187 |  |
| Matched | POST /api/Auth/update-login-password | 1 | app/core/services/auth-api.ts:193 | AuthController.SetLoginPassword |
| Matched | POST /api/Auth/validate-forgot-password-otp | 1 | app/core/services/auth-api.ts:181 | AuthController.ValidateForgotPasswordOtp |
| Matched | GET /api/ClientInfo/detect-device | 1 | app/core/services/auth-api.ts:161 | ClientInfoController.GetDeviceInfo |
| Matched | GET /api/CommonMenu | 1 | app/core/services/common-menu-api.ts:19 | CommonMenuController.Get |
| Matched | POST /api/Department/add | 1 | app/core/services/departments-api.ts:26 | DepartmentController.CreateDepartmentAsync |
| Matched | DELETE /api/Department/delete | 1 | app/core/services/departments-api.ts:53 | DepartmentController.DeleteDepartmentAsync |
| Matched | GET /api/Department/get | 1 | app/core/services/departments-api.ts:33 | DepartmentController.GetAllDepartmentsAsync |
| Matched | GET /api/Department/option | 1 | app/core/services/departments-api.ts:40 | DepartmentController.getDepartment |
| Matched | PUT /api/Department/update | 1 | app/core/services/departments-api.ts:46 | DepartmentController.UpdateDepartmentAsync |
| Matched | POST /api/Designation/add | 1 | app/core/services/designations-api.ts:55 | DesignationController.CreateDesignation |
| Matched | DELETE /api/Designation/delete | 1 | app/core/services/designations-api.ts:82 | DesignationController.Delete |
| Matched | GET /api/Designation/get | 1 | app/core/services/designations-api.ts:62 | DesignationController.GetAllDesignationAsyc |
| Matched | GET /api/Designation/option | 1 | app/core/services/designations-api.ts:69 | DesignationController.getDesignation |
| Matched | PUT /api/Designation/update | 1 | app/core/services/designations-api.ts:75 | DesignationController.UpdateDesignation |
| Matched | POST /api/DeviceMaster/create | 1 | app/core/services/device-master-api.ts:39 | DeviceMasterController.Create |
| Matched | DELETE /api/DeviceMaster/delete/{} | 1 | app/core/services/device-master-api.ts:58 | DeviceMasterController.Delete |
| Matched | GET /api/DeviceMaster/get-all | 1 | app/core/services/device-master-api.ts:27 | DeviceMasterController.GetAll |
| Matched | GET /api/DeviceMaster/get-by-id/{} | 1 | app/core/services/device-master-api.ts:33 | DeviceMasterController.GetById |
| Matched | POST /api/DeviceMaster/update | 1 | app/core/services/device-master-api.ts:46 | DeviceMasterController.Update |
| Matched | POST /api/DeviceMaster/update-status | 1 | app/core/services/device-master-api.ts:52 | DeviceMasterController.UpdateStatus |
| Route missing in backend | POST /api/Employee/assign-role | 1 | app/core/services/employee-api.ts:161 |  |
| Matched | POST /api/Employee/Bank/create | 1 | app/core/services/employee-banks-api.ts:85 | BankController.CreateBankInfo |
| Matched | DELETE /api/Employee/Bank/delete | 1 | app/core/services/employee-banks-api.ts:105 | BankController.Delete |
| Matched | GET /api/Employee/Bank/get | 1 | app/core/services/employee-banks-api.ts:92 | BankController.GetBankinfo |
| Matched | POST /api/Employee/Bank/update | 1 | app/core/services/employee-banks-api.ts:98 | BankController.Update |
| Matched | POST /api/Employee/Contact/create | 1 | app/core/services/employee-contacts-api.ts:75 | ContactController.CreateContactInfo |
| Matched | DELETE /api/Employee/Contact/delete | 1 | app/core/services/employee-contacts-api.ts:95 | ContactController.Delete |
| Matched | GET /api/Employee/Contact/get | 1 | app/core/services/employee-contacts-api.ts:82 | ContactController.GetBankinfo |
| Matched | POST /api/Employee/Contact/update | 1 | app/core/services/employee-contacts-api.ts:88 | ContactController.UpdateContact |
| Matched | POST /api/Employee/create | 1 | app/core/services/employee-api.ts:91 | EmployeeController.CreateEmployee |
| Matched | DELETE /api/Employee/delete-all | 1 | app/core/services/employee-api.ts:111 | EmployeeController.Delete |
| Matched | POST /api/Employee/Dependent/create | 1 | app/core/services/employee-dependent-api.ts:68 | DependentController.CreateDependentInfo |
| Matched | DELETE /api/Employee/Dependent/delete | 1 | app/core/services/employee-dependent-api.ts:99 | DependentController.Delete |
| Matched | GET /api/Employee/Dependent/get | 1 | app/core/services/employee-dependent-api.ts:75 | DependentController.Getinfo |
| Matched | GET /api/Employee/Dependent/get-in-detail | 1 | app/core/services/employee-dependent-api.ts:84 | DependentController.GetInDetail |
| Matched | POST /api/Employee/Dependent/update | 1 | app/core/services/employee-dependent-api.ts:92 | DependentController.Update |
| Matched | POST /api/Employee/Education/create | 1 | app/core/services/employee-education-api.ts:77 | EducationController.CreateEmployee |
| Matched | DELETE /api/Employee/Education/delete | 1 | app/core/services/employee-education-api.ts:97 | EducationController.Delete |
| Matched | GET /api/Employee/Education/get | 1 | app/core/services/employee-education-api.ts:84 | EducationController.GetAllEmployeeInfo |
| Matched | POST /api/Employee/Education/update-education | 1 | app/core/services/employee-education-api.ts:90 | EducationController.UpdateEducation |
| Matched | POST /api/Employee/Experience/create | 1 | app/core/services/employee-experience-api.ts:100 | ExperienceController.CreateExperience |
| Matched | DELETE /api/Employee/Experience/delete | 1 | app/core/services/employee-experience-api.ts:113 | ExperienceController.Delete |
| Matched | DELETE /api/Employee/Experience/delete-doc | 1 | app/core/services/employee-experience-api.ts:119 | ExperienceController.DeleteDoc |
| Matched | GET /api/Employee/Experience/get | 1 | app/core/services/employee-experience-api.ts:94 | ExperienceController.GetAllexperinceInfo |
| Matched | POST /api/Employee/Experience/update | 1 | app/core/services/employee-experience-api.ts:106 | ExperienceController.Update |
| Matched | GET /api/Employee/get | 1 | app/core/services/employee-basic-api.ts:132 | EmployeeController.GetEmployee |
| Matched | GET /api/Employee/get-all | 1 | app/core/services/employee-api.ts:98 | EmployeeController.GetAllEmployee |
| Matched | GET /api/Employee/get-all-percentage | 1 | app/core/services/employee-api.ts:118 | EmployeeController.GetAllEmployeePercentageAsync |
| Matched | GET /api/Employee/get-profile-summary | 2 | app/core/services/employee-basic-api.ts:154<br>app/core/services/employee-basic-api.ts:182 | EmployeeController.GetEmployeeProfileSummary |
| Matched | GET /api/Employee/get-summary | 1 | app/core/services/employee-api.ts:138 | EmployeeController.GetEmployeeSummary |
| Matched | GET /api/Employee/Image/get | 1 | app/core/services/employee-basic-api.ts:169 | EmployeeController.GetAllEmployeeImage |
| Matched | DELETE /api/Employee/Insurance/delete | 1 | app/core/services/employee-insurance-api.ts:79 | InsuranceController.DeleteEnrolledEmployee |
| Matched | POST /api/Employee/Insurance/employee-insurance-enroll | 1 | app/core/services/employee-insurance-api.ts:66 | InsuranceController.EnrolledEmployee |
| Matched | GET /api/Employee/Insurance/get-all-enroll | 1 | app/core/services/employee-insurance-api.ts:73 | InsuranceController.Get |
| Matched | POST /api/Employee/official/update | 1 | app/core/services/employee-basic-api.ts:146 | EmployeeController.OfficialUpdate |
| Matched | POST /api/Employee/profile/pic/update | 1 | app/core/services/employee-basic-api.ts:175 | EmployeeController.UpdateProfieImage |
| Matched | POST /api/Employee/Sensitive/create | 1 | app/core/services/employee-identity-api.ts:72 | SensitiveController.Createpersonalinfo |
| Matched | GET /api/Employee/Sensitive/get | 1 | app/core/services/employee-identity-api.ts:66 | SensitiveController.GetSensitiveData |
| Route missing in backend | POST /api/Employee/Sensitive/update | 1 | app/core/services/employee-identity-api.ts:78 |  |
| Matched | POST /api/Employee/update | 2 | app/core/services/employee-api.ts:104<br>app/core/services/employee-basic-api.ts:138 | EmployeeController.Update |
| Matched | POST /api/Employee/update-bulk | 1 | app/core/services/employee-api.ts:124 | EmployeeController.UpdateSectionStatusBulk |
| Matched | POST /api/Employee/update-edit-status | 1 | app/core/services/profile-access-api.ts:47 | EmployeeController.UpdateSectionStatusBulk |
| Matched | PUT /api/Employee/update-status | 1 | app/core/services/employee-api.ts:131 | EmployeeController.UpdateEmployeeStatus |
| Matched | POST /api/Employee/update-verification-status | 1 | app/core/services/profile-access-api.ts:41 | EmployeeController.UpdateVerificationStatus |
| Route missing in backend | POST /api/EmployeeDeviceEnrollment/create | 1 | app/core/services/employee-device-enrollment-api.ts:50 |  |
| Route missing in backend | DELETE /api/EmployeeDeviceEnrollment/delete/{} | 1 | app/core/services/employee-device-enrollment-api.ts:81 |  |
| Route missing in backend | GET /api/EmployeeDeviceEnrollment/get-all | 1 | app/core/services/employee-device-enrollment-api.ts:30 |  |
| Route missing in backend | GET /api/EmployeeDeviceEnrollment/get-by-id/{} | 1 | app/core/services/employee-device-enrollment-api.ts:40 |  |
| Route missing in backend | POST /api/EmployeeDeviceEnrollment/update | 1 | app/core/services/employee-device-enrollment-api.ts:61 |  |
| Route missing in backend | POST /api/EmployeeDeviceEnrollment/update-status | 1 | app/core/services/employee-device-enrollment-api.ts:71 |  |
| Route missing in backend | POST /api/EmployeeLocationAssignment/create | 1 | app/core/services/employee-location-assignment-api.ts:52 |  |
| Route missing in backend | DELETE /api/EmployeeLocationAssignment/delete/{} | 1 | app/core/services/employee-location-assignment-api.ts:85 |  |
| Route missing in backend | GET /api/EmployeeLocationAssignment/get-all | 1 | app/core/services/employee-location-assignment-api.ts:30 |  |
| Route missing in backend | GET /api/EmployeeLocationAssignment/get-by-id/{} | 1 | app/core/services/employee-location-assignment-api.ts:40 |  |
| Route missing in backend | POST /api/EmployeeLocationAssignment/update | 1 | app/core/services/employee-location-assignment-api.ts:63 |  |
| Route missing in backend | POST /api/EmployeeLocationAssignment/update-status | 1 | app/core/services/employee-location-assignment-api.ts:75 |  |
| Matched | GET /api/EmployeeType/get | 1 | app/core/services/employee-types-api.ts:49 | EmployeeTypeController.GetAllEmployeeType |
| Matched | GET /api/EmployeeType/option | 1 | app/core/services/employee-types-api.ts:56 | EmployeeTypeController.GetAllEmployeeType |
| Route missing in backend | POST /api/EmployeeWorkArrangement/create | 1 | app/core/services/employee-work-arrangement-api.ts:48 |  |
| Route missing in backend | DELETE /api/EmployeeWorkArrangement/delete/{} | 1 | app/core/services/employee-work-arrangement-api.ts:80 |  |
| Route missing in backend | GET /api/EmployeeWorkArrangement/get-all | 1 | app/core/services/employee-work-arrangement-api.ts:30 |  |
| Route missing in backend | GET /api/EmployeeWorkArrangement/get-by-id/{} | 1 | app/core/services/employee-work-arrangement-api.ts:38 |  |
| Route missing in backend | POST /api/EmployeeWorkArrangement/update | 1 | app/core/services/employee-work-arrangement-api.ts:59 |  |
| Route missing in backend | POST /api/EmployeeWorkArrangement/update-status | 1 | app/core/services/employee-work-arrangement-api.ts:70 |  |
| Route missing in backend | POST /api/EmployeeWorkModeOverride/create | 1 | app/core/services/employee-work-mode-override-api.ts:50 |  |
| Route missing in backend | DELETE /api/EmployeeWorkModeOverride/delete/{} | 1 | app/core/services/employee-work-mode-override-api.ts:81 |  |
| Route missing in backend | GET /api/EmployeeWorkModeOverride/get-all | 1 | app/core/services/employee-work-mode-override-api.ts:30 |  |
| Route missing in backend | GET /api/EmployeeWorkModeOverride/get-by-id/{} | 1 | app/core/services/employee-work-mode-override-api.ts:40 |  |
| Route missing in backend | POST /api/EmployeeWorkModeOverride/update | 1 | app/core/services/employee-work-mode-override-api.ts:61 |  |
| Route missing in backend | POST /api/EmployeeWorkModeOverride/update-status | 1 | app/core/services/employee-work-mode-override-api.ts:71 |  |
| Route missing in backend | POST /api/EmployeeWorkPattern/create | 1 | app/core/services/employee-work-pattern-api.ts:42 |  |
| Route missing in backend | DELETE /api/EmployeeWorkPattern/delete/{} | 1 | app/core/services/employee-work-pattern-api.ts:67 |  |
| Route missing in backend | GET /api/EmployeeWorkPattern/get-all | 1 | app/core/services/employee-work-pattern-api.ts:28 |  |
| Route missing in backend | GET /api/EmployeeWorkPattern/get-by-id/{} | 1 | app/core/services/employee-work-pattern-api.ts:34 |  |
| Route missing in backend | POST /api/EmployeeWorkPattern/update | 1 | app/core/services/employee-work-pattern-api.ts:51 |  |
| Route missing in backend | POST /api/EmployeeWorkPattern/update-status | 1 | app/core/services/employee-work-pattern-api.ts:59 |  |
| Route missing in backend | GET /api/Gender/Gender/get | 1 | app/core/services/users-api.ts:37 |  |
| Matched | GET /api/Gender/option | 1 | app/core/services/users-api.ts:44 | GenderController.getGender |
| Matched | POST /api/Host/change-host-user-password | 1 | app/core/services/host-api.ts:95 | HostController.ChangeHostUserPassword |
| Matched | POST /api/Host/create-host-role | 1 | app/core/services/host-api.ts:101 | HostController.CreateHostRole |
| Matched | POST /api/Host/create-host-user | 1 | app/core/services/host-api.ts:71 | HostController.CreateHostUser |
| Matched | POST /api/Host/delete-host-role | 1 | app/core/services/host-api.ts:113 | HostController.DeleteHostRole |
| Matched | POST /api/Host/delete-host-user | 1 | app/core/services/host-api.ts:83 | HostController.DeleteHostUser |
| Matched | GET /api/Host/get-all-host-roles | 1 | app/core/services/host-api.ts:51 | HostController.GetAllHostRoles |
| Matched | GET /api/Host/get-all-host-users | 1 | app/core/services/host-api.ts:37 | HostController.GetAllHostUsers |
| Matched | GET /api/Host/get-host-modules | 1 | app/core/services/host-api.ts:65 | HostController.GetHostModules |
| Matched | GET /api/Host/get-host-role-by-id/{} | 1 | app/core/services/host-api.ts:57 | HostController.GetHostRoleById |
| Matched | GET /api/Host/get-host-user-by-id/{} | 1 | app/core/services/host-api.ts:45 | HostController.GetHostUserById |
| Matched | POST /api/Host/reset-host-user-password | 1 | app/core/services/host-api.ts:89 | HostController.ResetHostUserPassword |
| Matched | POST /api/Host/update-host-role | 1 | app/core/services/host-api.ts:107 | HostController.UpdateHostRole |
| Matched | POST /api/Host/update-host-user | 1 | app/core/services/host-api.ts:77 | HostController.UpdateHostUser |
| Matched | GET /api/HostRolePermission/get-role-module-permissions/{} | 1 | app/core/services/host-api.ts:124 | HostRolePermissionController.GetRoleModulePermissions |
| Matched | POST /api/HostRolePermission/save-role-module-permissions | 1 | app/core/services/host-api.ts:139 | HostRolePermissionController.SaveRoleModulePermissions |
| Matched | POST /api/Insurance/create | 1 | app/core/services/policies-insurance-api.ts:65 | InsuranceController.Create |
| Matched | DELETE /api/Insurance/delete | 1 | app/core/services/policies-insurance-api.ts:92 | InsuranceController.Delete |
| Matched | GET /api/Insurance/get-all | 1 | app/core/services/policies-insurance-api.ts:72 | InsuranceController.GetList |
| Matched | GET /api/Insurance/get-ddl | 1 | app/core/services/policies-insurance-api.ts:79 | InsuranceController.GetList |
| Matched | GET /api/Insurance/get-detail-ddl | 1 | app/core/services/policies-insurance-api.ts:99 | InsuranceController.GetDetailList |
| Matched | PUT /api/Insurance/update | 1 | app/core/services/policies-insurance-api.ts:85 | InsuranceController.Update |
| Route missing in backend | POST /api/Leave/add-leave-type | 1 | app/core/services/leave-types.ts:44 |  |
| Route missing in backend | POST /api/Leave/delete-leave-type | 1 | app/core/services/leave-types.ts:56 |  |
| Route missing in backend | GET /api/Leave/get-all-leave-type | 1 | app/core/services/leave-types.ts:38 |  |
| Route missing in backend | POST /api/Leave/update-leave-type | 1 | app/core/services/leave-types.ts:50 |  |
| Matched | POST /api/LeaveRule/create | 1 | app/core/services/leave-rule.ts:42 | LeaveRuleController.CreateLeaveRuleAsync |
| Matched | POST /api/LeaveRule/delete | 1 | app/core/services/leave-rule.ts:54 | LeaveRuleController.DeleteLeavePolicy |
| Matched | GET /api/LeaveRule/get | 1 | app/core/services/leave-rule.ts:36 | LeaveRuleController.GetAllLeaveRuleAsync |
| Matched | POST /api/LeaveRule/update | 1 | app/core/services/leave-rule.ts:48 | LeaveRuleController.UpdateLeavePolicyAsync |
| Matched | GET /api/Location/country/option | 1 | app/core/services/locations-api.ts:60 | LocationController.getCountry |
| Matched | GET /api/Location/district/option | 1 | app/core/services/locations-api.ts:74 | LocationController.getDistrict |
| Matched | GET /api/Location/State/option | 1 | app/core/services/locations-api.ts:67 | LocationController.getState |
| Matched | POST /api/ModuleOperation/create | 1 | app/core/services/module-operation-api.ts:37 | ModuleOperationController.CreateModuleOperation |
| Matched | DELETE /api/ModuleOperation/delete/{} | 1 | app/core/services/module-operation-api.ts:50 | ModuleOperationController.DeleteModuleOperation |
| Matched | GET /api/ModuleOperation/get-all | 1 | app/core/services/module-operation-api.ts:25 | ModuleOperationController.GetAllModuleOperations |
| Matched | GET /api/ModuleOperation/get-by-id/{} | 1 | app/core/services/module-operation-api.ts:31 | ModuleOperationController.GetModuleOperationById |
| Matched | POST /api/ModuleOperation/update | 1 | app/core/services/module-operation-api.ts:44 | ModuleOperationController.UpdateModuleOperation |
| Matched | POST /api/NewLogin/login | 1 | app/core/services/auth-api.ts:153 | NewLoginController.Login |
| Matched | POST /api/OperationsMaster/create-operation | 1 | app/core/services/operations-master-api.ts:37 | OperationsMasterController.CreateOperation |
| Matched | DELETE /api/OperationsMaster/delete-operation/{} | 1 | app/core/services/operations-master-api.ts:50 | OperationsMasterController.DeleteOperation |
| Matched | GET /api/OperationsMaster/get-all-operations | 1 | app/core/services/operations-master-api.ts:25 | OperationsMasterController.GetAllOperations |
| Matched | GET /api/OperationsMaster/get-operation/{} | 1 | app/core/services/operations-master-api.ts:31 | OperationsMasterController.GetOperationById |
| Matched | POST /api/OperationsMaster/update-operation | 1 | app/core/services/operations-master-api.ts:44 | OperationsMasterController.UpdateOperation |
| Matched | POST /api/Option/create | 1 | app/core/services/option-api.ts:32 | OptionController.CreateOperation |
| Matched | GET /api/Option/get | 1 | app/core/services/option-api.ts:26 | OptionController.GetAllOperationAsyc |
| Matched | GET /api/Option/has-access | 1 | app/core/services/option-api.ts:45 | OptionController.HasPageOperationAccess |
| Matched | POST /api/Option/update | 1 | app/core/services/option-api.ts:38 | OptionController.UpdateOperation |
| Matched | GET /api/ParentModule/{} | 1 | app/core/services/parent-module-api.ts:35 | ParentModuleController.GetModuleById |
| Matched | PUT /api/ParentModule/{} | 1 | app/core/services/parent-module-api.ts:47 | ParentModuleController.UpdateModule |
| Matched | PATCH /api/ParentModule/{}/status | 1 | app/core/services/parent-module-api.ts:53 | ParentModuleController.UpdateModuleStatus |
| Matched | POST /api/ParentModule/add | 1 | app/core/services/parent-module-api.ts:41 | ParentModuleController.AddModule |
| Matched | GET /api/ParentModule/get-module-headers | 1 | app/core/services/parent-module-api.ts:28 | ParentModuleController.GetModuleHeaders |
| Method mismatch | GET /api/PlanModuleMapping/save | 1 | app/core/services/plan-module-mapping-api.ts:24 | Backend verb(s): POST |
| Matched | POST /api/PlanModuleMapping/save | 1 | app/core/services/plan-module-mapping-api.ts:32 | PlanModuleMappingController.Save |
| Matched | POST /api/PolicyMappingLeaveType/delete | 1 | app/core/services/policy-mapping-leave-type-api.ts:103 | PolicyMappingLeaveTypeController.DeleteLeavePolicy |
| Matched | GET /api/PolicyMappingLeaveType/get | 1 | app/core/services/policy-mapping-leave-type-api.ts:46 | PolicyMappingLeaveTypeController.GetAllLeavePoliciesAsync |
| Route missing in backend | GET /api/PolicyMappingLeaveType/get-by-employee-type-Id | 1 | app/core/services/policy-mapping-leave-type-api.ts:67 |  |
| Matched | POST /api/PolicyMappingLeaveType/map | 1 | app/core/services/policy-mapping-leave-type-api.ts:79 | PolicyMappingLeaveTypeController.CreateLeavePolicyAsync |
| Matched | POST /api/PolicyMappingLeaveType/update | 1 | app/core/services/policy-mapping-leave-type-api.ts:91 | PolicyMappingLeaveTypeController.UpdateLeavePolicyAsync |
| Matched | POST /api/PolicyType/create | 1 | app/core/services/policy-type-api.ts:91 | PolicyTypeController.CreatePolicyTypeAsync |
| Matched | DELETE /api/PolicyType/delete | 1 | app/core/services/policy-type-api.ts:113 | PolicyTypeController.DeletePolicyTypeAsync |
| Matched | GET /api/PolicyType/get-all | 1 | app/core/services/policy-type-api.ts:78 | PolicyTypeController.GetAllPolicyTypesAsync |
| Matched | GET /api/PolicyType/get-all-unstruct | 1 | app/core/services/policy-type-api.ts:106 | PolicyTypeController.GetUnstructuredPolicyTypesAsync |
| Matched | GET /api/PolicyType/get-ddl | 1 | app/core/services/policy-type-api.ts:85 | PolicyTypeController.GetDDLPolicyTypesAsync |
| Matched | POST /api/PolicyType/update | 1 | app/core/services/policy-type-api.ts:97 | PolicyTypeController.UpdatePolicyTypeAsync |
| Matched | DELETE /api/PolicyTypeInsuranceMap/delete | 1 | app/core/services/policy-type-insurance-map-api.ts:104 | PolicyTypeInsuranceMapController.Delete |
| Matched | GET /api/PolicyTypeInsuranceMap/get-all | 1 | app/core/services/policy-type-insurance-map-api.ts:60 | PolicyTypeInsuranceMapController.GetList |
| Matched | GET /api/PolicyTypeInsuranceMap/get-details | 1 | app/core/services/policy-type-insurance-map-api.ts:70 | PolicyTypeInsuranceMapController.GetDetailList |
| Matched | POST /api/PolicyTypeInsuranceMap/map | 1 | app/core/services/policy-type-insurance-map-api.ts:85 | PolicyTypeInsuranceMapController.Create |
| Matched | PUT /api/PolicyTypeInsuranceMap/update | 1 | app/core/services/policy-type-insurance-map-api.ts:93 | PolicyTypeInsuranceMapController.Update |
| Matched | POST /api/ReportingType/create | 1 | app/core/services/report-type-api.ts:31 | ReportingTypeController.CreateReportingType |
| Matched | DELETE /api/ReportingType/delete | 1 | app/core/services/report-type-api.ts:44 | ReportingTypeController.DeleteReportingType |
| Matched | GET /api/ReportingType/get-all | 1 | app/core/services/report-type-api.ts:25 | ReportingTypeController.GetAllReportingTypes |
| Matched | PUT /api/ReportingType/update | 1 | app/core/services/report-type-api.ts:37 | ReportingTypeController.UpdateReportingType |
| Matched | POST /api/Role/add | 1 | app/core/services/roles-api.ts:101 | RoleController.CreateRole |
| Matched | DELETE /api/Role/delete | 1 | app/core/services/roles-api.ts:128 | RoleController.DeleteRole |
| Matched | GET /api/Role/get | 1 | app/core/services/roles-api.ts:108 | RoleController.GetAllRoles |
| Route missing in backend | GET /api/Role/get-permissions | 2 | app/core/services/employee-api.ts:153<br>app/core/services/roles-api.ts:135 |  |
| Matched | GET /api/Role/option | 1 | app/core/services/roles-api.ts:115 | RoleController.getRole |
| Matched | PUT /api/Role/update | 1 | app/core/services/roles-api.ts:121 | RoleController.UpdateRole |
| Matched | GET /api/StatData/Dashboard/Employees/Statistics | 1 | app/core/services/employee-api.ts:146 | StatDataController.GetEmployeeDashboardSummaryAsync |
| Matched | GET /api/StatData/Manager/Statistic/Asset | 1 | app/core/services/dashboard-api.ts:63 | StatDataController.Asset |
| Matched | GET /api/StatData/Manager/Statistics/Dashboard/get | 1 | app/core/services/dashboard-api.ts:56 | StatDataController.dashboard |
| Matched | GET /api/SubModule/{} | 1 | app/core/services/sub-module-api.ts:34 | SubModuleController.GetModuleById |
| Matched | PUT /api/SubModule/{} | 1 | app/core/services/sub-module-api.ts:46 | SubModuleController.UpdateModule |
| Matched | PATCH /api/SubModule/{}/status | 1 | app/core/services/sub-module-api.ts:52 | SubModuleController.UpdateModuleStatus |
| Matched | POST /api/SubModule/add | 1 | app/core/services/sub-module-api.ts:40 | SubModuleController.AddModule |
| Matched | GET /api/SubModule/list | 1 | app/core/services/sub-module-api.ts:27 | SubModuleController.GetModules |
| Method mismatch | GET /api/Subscription/{} | 3 | app/core/services/subscriptions-api.ts:36<br>app/core/services/subscriptions-api.ts:58<br>app/core/services/subscriptions-api.ts:70 | Backend verb(s): PUT |
| Method mismatch | POST /api/Subscription/{} | 3 | app/core/services/subscriptions-api.ts:49<br>app/core/services/subscriptions-api.ts:79<br>app/core/services/subscriptions-api.ts:86 | Backend verb(s): PUT |
| Matched | PUT /api/Subscription/{} | 1 | app/core/services/subscriptions-api.ts:94 | SubscriptionController.UpdateSubscription |
| Matched | POST /api/Tenant/{}/resend-verification | 1 | app/core/services/tenants-api.ts:171 | TenantController.ResendTenantVerificationAsync |
| Matched | POST /api/Tenant/activate-tenant | 1 | app/core/services/tenants-api.ts:152 | TenantController.ActivateTenantAsync |
| Matched | POST /api/Tenant/create-tenant | 1 | app/core/services/tenants-api.ts:98 | TenantController.TenantCreation |
| Matched | POST /api/Tenant/deactivate-tenant | 1 | app/core/services/tenants-api.ts:158 | TenantController.DeactivateTenantAsync |
| Matched | POST /api/Tenant/delete-tenant | 1 | app/core/services/tenants-api.ts:164 | TenantController.DeleteTenantAsync |
| Matched | GET /api/Tenant/get-all-tenant-operations | 1 | app/core/services/tenants-api.ts:126 | TenantController.GetAllNodeLeafeWithOperationsAsync |
| Matched | GET /api/Tenant/get-all-tenants | 1 | app/core/services/tenants-api.ts:111 | TenantController.GetAllTenantsAsync |
| Matched | GET /api/Tenant/get-employee-code-pattern | 1 | app/core/services/tenants-api.ts:134 | TenantController.GetEmployeeCodePatternAsync |
| Matched | GET /api/Tenant/get-tenant-by-id | 1 | app/core/services/tenants-api.ts:119 | TenantController.GetTenantByIdAsync |
| Matched | POST /api/Tenant/update-modules-and-operations | 1 | app/core/services/tenants-api.ts:146 | TenantController.TenantModuleOperationsUpdate |
| Matched | POST /api/Tenant/update-tenant | 1 | app/core/services/tenants-api.ts:140 | TenantController.UpdateTenantAsync |
| Matched | POST /api/Tenant/verify | 1 | app/core/services/tenants-api.ts:104 | TenantController.VerifyEmail |
| Matched | POST /api/TenantDevice/create | 1 | app/core/services/tenant-device-api.ts:39 | TenantDeviceController.Create |
| Matched | DELETE /api/TenantDevice/delete/{} | 1 | app/core/services/tenant-device-api.ts:58 | TenantDeviceController.Delete |
| Matched | GET /api/TenantDevice/get-all | 1 | app/core/services/tenant-device-api.ts:27 | TenantDeviceController.GetAll |
| Matched | GET /api/TenantDevice/get-by-id/{} | 1 | app/core/services/tenant-device-api.ts:33 | TenantDeviceController.GetById |
| Matched | POST /api/TenantDevice/update | 1 | app/core/services/tenant-device-api.ts:46 | TenantDeviceController.Update |
| Matched | POST /api/TenantDevice/update-status | 1 | app/core/services/tenant-device-api.ts:52 | TenantDeviceController.UpdateStatus |
| Matched | GET /api/TenantIndustry/get-industries | 1 | app/core/services/industries-api.ts:42 | TenantIndustryController.GetAllTenantBySubscriptionIdAsync |
| Matched | GET /api/TenantIndustry/get-tenant-subscription-plan | 1 | app/core/services/industries-api.ts:52 | TenantIndustryController.GetTenantSubscriptionPlanInfoAsync |
| Route missing in backend | POST /api/TenantLocation/create | 1 | app/core/services/tenant-location-api.ts:40 |  |
| Route missing in backend | DELETE /api/TenantLocation/delete/{} | 1 | app/core/services/tenant-location-api.ts:62 |  |
| Route missing in backend | GET /api/TenantLocation/get-all | 1 | app/core/services/tenant-location-api.ts:28 |  |
| Route missing in backend | GET /api/TenantLocation/get-by-id/{} | 1 | app/core/services/tenant-location-api.ts:34 |  |
| Route missing in backend | POST /api/TenantLocation/update | 1 | app/core/services/tenant-location-api.ts:47 |  |
| Route missing in backend | POST /api/TenantLocation/update-status | 1 | app/core/services/tenant-location-api.ts:56 |  |
| Matched | POST /api/Ticket/TicketHeader/create | 1 | app/features/tickets/ticket-api.ts:85 | TicketHeaderController.CreateHeader |
| Matched | DELETE /api/Ticket/TicketHeader/delete | 1 | app/features/tickets/ticket-api.ts:97 | TicketHeaderController.DeleteTicketHeader |
| Route missing in backend | GET /api/Ticket/TicketHeader/get-all | 1 | app/features/tickets/ticket-api.ts:79 (conditional URL branch) |  |
| Matched | GET /api/Ticket/TicketHeader/get-by-classification-id | 1 | app/features/tickets/ticket-api.ts:79 (conditional URL branch) | TicketHeaderController.GetAllHeaderFilterAsync |
| Matched | PUT /api/Ticket/TicketHeader/update | 1 | app/features/tickets/ticket-api.ts:91 | TicketHeaderController.UpdateHeader |
| Matched | POST /api/Ticket/TicketType/create | 1 | app/features/tickets/ticket-api.ts:113 | TicketTypeController.CreateTicketType |
| Matched | GET /api/Ticket/TicketType/ddl-list | 1 | app/features/tickets/ticket-api.ts:131 | TicketTypeController.GetDDLTicketTypes |
| Matched | DELETE /api/Ticket/TicketType/delete | 1 | app/features/tickets/ticket-api.ts:125 | TicketTypeController.DeleteTicketType |
| Matched | GET /api/Ticket/TicketType/get-all | 1 | app/features/tickets/ticket-api.ts:107 (conditional URL branch) | TicketTypeController.GetAllTicketTypes |
| Matched | GET /api/Ticket/TicketType/get-by-header-id | 1 | app/features/tickets/ticket-api.ts:107 (conditional URL branch) | TicketTypeController.GetTicketTypesByHeaderId |
| Matched | PUT /api/Ticket/TicketType/update | 1 | app/features/tickets/ticket-api.ts:119 | TicketTypeController.UpdateTicketType |
| Matched | GET /api/TicketClassification/all | 1 | app/features/tickets/ticket-api.ts:45 | TicketClassificationController.GetAllTicketClassifications |
| Matched | POST /api/TicketClassification/create | 1 | app/features/tickets/ticket-api.ts:53 | TicketClassificationController.CreateTicketClassification |
| Matched | GET /api/TicketClassification/ddl-list | 1 | app/features/tickets/ticket-api.ts:34 | TicketClassificationController.GetAllTicketClassifications |
| Matched | DELETE /api/TicketClassification/delete | 1 | app/features/tickets/ticket-api.ts:69 | TicketClassificationController.DeleteTicketClassification |
| Matched | PUT /api/TicketClassification/update | 1 | app/features/tickets/ticket-api.ts:61 | TicketClassificationController.UpdateTicketClassification |
| Route missing in backend | POST /api/TicketCreation/open | 1 | app/features/tickets/ticket-api.ts:139 |  |
| Route missing in backend | POST /api/UserModuleRolePermission/assign-employee-permissions | 1 | app/core/services/employee-api.ts:178 |  |
| Matched | POST /api/UserModuleRolePermission/assign-role-permissions | 1 | app/core/services/roles-api.ts:151 | UserModuleRolePermissionController.CreatePermission |
| Matched | GET /api/UserModuleRolePermission/get-role-based-permissions | 1 | app/core/services/roles-api.ts:142 | UserModuleRolePermissionController.GetTenantEnabledOperations |
| Matched | POST /api/UserRole/assign-roles-to-user | 1 | app/core/services/roles-api.ts:167 | UserRoleController.CreatePermission |
| Matched | GET /api/UserRole/get-all-user-roles | 1 | app/core/services/roles-api.ts:161 | UserRoleController.GetTenantEnabledOperations |

## Backend controller summary

| Controller | API endpoints | Used by Angular | No exact Angular call |
|---|---:|---:|---:|
| AssetController (axionpro.api/Controllers/Asset/AssetController.cs) | 4 | 4 | 0 |
| AttendanceController (axionpro.api/Controllers/Attendance/AttendanceController.cs) | 2 | 0 | 2 |
| AuthController (axionpro.api/Controllers/Login/AuthController.cs) | 7 | 6 | 1 |
| BankController (axionpro.api/Controllers/Employee/BankController.cs) | 4 | 4 | 0 |
| CategoryController (axionpro.api/Controllers/Asset/CategoryController.cs) | 4 | 4 | 0 |
| CategoryController (axionpro.api/Controllers/Category/CategoryController.cs) | 2 | 0 | 2 |
| ClientController (axionpro.api/Controllers/Client/ClientController.cs) | 3 | 0 | 3 |
| ClientInfoController (axionpro.api/Controllers/ClientInfo/ClientInfoController.cs) | 1 | 1 | 0 |
| CommonMenuController (axionpro.api/Controllers/CommonMenu/CommonMenuController.cs) | 1 | 1 | 0 |
| CommonModuleController (axionpro.api/Controllers/Module/CommonModuleController.cs) | 1 | 0 | 1 |
| CompanyController (axionpro.api/Controllers/Company/CompanyController.cs) | 1 | 0 | 1 |
| ComplianceRuleController (axionpro.api/Controllers/Compliance/ComplianceRuleController.cs) | 1 | 0 | 1 |
| ContactController (axionpro.api/Controllers/Employee/ContactController.cs) | 4 | 4 | 0 |
| DepartmentController (axionpro.api/Controllers/Department/DepartmentController.cs) | 5 | 5 | 0 |
| DependentController (axionpro.api/Controllers/Employee/DependentController.cs) | 5 | 5 | 0 |
| DesignationController (axionpro.api/Controllers/Designation/DesignationController.cs) | 6 | 5 | 1 |
| DeviceMasterController (axionpro.api/Controllers/HostDevice/DeviceMasterController.cs) | 6 | 6 | 0 |
| EducationController (axionpro.api/Controllers/Employee/EducationController.cs) | 4 | 4 | 0 |
| EmailTemplateController (axionpro.api/Controllers/EmailTemplate/EmailTemplateController.cs) | 2 | 0 | 2 |
| EmployeeController (axionpro.api/Controllers/Employee/EmployeeController.cs) | 15 | 15 | 0 |
| EmployeeLeavePolicyController (axionpro.api/Controllers/Leave/EmployeeLeavePolicyController.cs) | 6 | 0 | 6 |
| EmployeeTypeController (axionpro.api/Controllers/EmployeeType/EmployeeTypeController.cs) | 2 | 2 | 0 |
| EntityController (axionpro.api/Controllers/Entity/EntityController.cs) | 1 | 0 | 1 |
| EnumController (axionpro.api/Controllers/EnumTypes/EnumController.cs) | 1 | 0 | 1 |
| ExperienceController (axionpro.api/Controllers/Employee/ExperienceController.cs) | 5 | 5 | 0 |
| FileUploadController (axionpro.api/Controllers/FileUpload/FileUploadController.cs) | 1 | 0 | 1 |
| GenderController (axionpro.api/Controllers/Gender/GenderController.cs) | 2 | 1 | 1 |
| HolidayCalandarController (axionpro.api/Controllers/HolidayCalandar/HolidayCalandarController.cs) | 1 | 0 | 1 |
| HostAccessController (axionpro.api/Controllers/Host/HostAccessController.cs) | 1 | 0 | 1 |
| HostController (axionpro.api/Controllers/Host/HostController.cs) | 14 | 13 | 1 |
| HostRolePermissionController (axionpro.api/Controllers/Host/HostRolePermissionController.cs) | 2 | 2 | 0 |
| InsuranceController (axionpro.api/Controllers/Employee/InsuranceController.cs) | 3 | 3 | 0 |
| InsuranceController (axionpro.api/Controllers/Insurance/InsuranceController.cs) | 6 | 6 | 0 |
| LeaveController (axionpro.api/Controllers/Leave/LeaveController.cs) | 4 | 0 | 4 |
| LeaveRuleController (axionpro.api/Controllers/Leave/LeaveRuleController.cs) | 5 | 4 | 1 |
| LocationController (axionpro.api/Controllers/Location/LocationController.cs) | 3 | 3 | 0 |
| MenuStructureController (axionpro.api/Controllers/MenuStructureView/MenuStructureController.cs) | 1 | 0 | 1 |
| ModuleOperationController (axionpro.api/Controllers/Module/ModuleOperationController.cs) | 5 | 5 | 0 |
| NewLoginController (axionpro.api/Controllers/Login/NewLoginController.cs) | 1 | 1 | 0 |
| OperationsMasterController (axionpro.api/Controllers/OperationsMaster/OperationsMasterController.cs) | 5 | 5 | 0 |
| OptionController (axionpro.api/Controllers/Operation/OperationController.cs) | 4 | 4 | 0 |
| ParentModuleController (axionpro.api/Controllers/Module/ParentModuleController.cs) | 5 | 5 | 0 |
| PlanModuleMappingController (axionpro.api/Controllers/Subscription/PlanModuleMappingController.cs) | 2 | 1 | 1 |
| PolicyMappingLeaveTypeController (axionpro.api/Controllers/Leave/PolicyMappingLeaveTypeController.cs) | 5 | 4 | 1 |
| PolicyTypeController (axionpro.api/Controllers/Policies/PolicyTypeController.cs) | 7 | 6 | 1 |
| PolicyTypeInsuranceMapController (axionpro.api/Controllers/Insurance/PolicyTypeInsuranceMapController.cs) | 6 | 5 | 1 |
| RegistrationController (axionpro.api/Controllers/Registration/RegistrationController.cs) | 2 | 0 | 2 |
| ReportingTypeController (axionpro.api/Controllers/ReportingType/ReportingTypeController.cs) | 5 | 4 | 1 |
| RoleController (axionpro.api/Controllers/Role/RoleController.cs) | 5 | 5 | 0 |
| RuleController (axionpro.api/Controllers/SandwichRule/RuleController.cs) | 8 | 0 | 8 |
| SensitiveController (axionpro.api/Controllers/Employee/SensitiveController.cs) | 2 | 2 | 0 |
| StatDataController (axionpro.api/Controllers/Stats/StatDataController.cs) | 3 | 3 | 0 |
| StatusController (axionpro.api/Controllers/Asset/StatusController.cs) | 4 | 4 | 0 |
| SubModuleController (axionpro.api/Controllers/Module/SubModuleController.cs) | 6 | 5 | 1 |
| SubscriptionController (axionpro.api/Controllers/Subscription/SubscriptionController.cs) | 7 | 1 | 6 |
| TenantController (axionpro.api/Controllers/Tenant/TenantController.cs) | 17 | 12 | 5 |
| TenantDeviceController (axionpro.api/Controllers/HostDevice/TenantDeviceController.cs) | 6 | 6 | 0 |
| TenantIndustryController (axionpro.api/Controllers/TenantIndustry/TenantIndustryController.cs) | 2 | 2 | 0 |
| TenantParentModuleController (axionpro.api/Controllers/Module/TenantParentModuleController.cs) | 4 | 0 | 4 |
| TenantUserAccessController (axionpro.api/Controllers/TenantUserAccess/TenantUserAccessController.cs) | 1 | 0 | 1 |
| TicketClassificationController (axionpro.api/Controllers/Ticket/ClassificationController.cs) | 6 | 5 | 1 |
| TicketHeaderController (axionpro.api/Controllers/Ticket/TicketHeaderController.cs) | 4 | 4 | 0 |
| TicketTypeController (axionpro.api/Controllers/Ticket/TicketTypeController.cs) | 7 | 6 | 1 |
| TravelController (axionpro.api/Controllers/Travel/TravelController.cs) | 3 | 0 | 3 |
| TypeController (axionpro.api/Controllers/Asset/TypeController.cs) | 4 | 4 | 0 |
| UserModuleRolePermissionController (axionpro.api/Controllers/UserModuleRolePermission/UserModuleRolePermissionController.cs) | 2 | 2 | 0 |
| UserRoleController (axionpro.api/Controllers/UserRole/UserRoleController.cs) | 2 | 2 | 0 |
| WorkflowStageController (axionpro.api/Controllers/WorkflowStage/WorkflowStageController.cs) | 5 | 0 | 5 |

## Backend API inventory

| Status | Request | Controller action(s) | Source(s) |
|---|---|---|---|
| Used by Angular | POST /api/Asset/add | AssetController.AddAsset | axionpro.api/Controllers/Asset/AssetController.cs:64 |
| Used by Angular | POST /api/Asset/Category/add | CategoryController.AddAssetCategory | axionpro.api/Controllers/Asset/CategoryController.cs:64 |
| Used by Angular | DELETE /api/Asset/Category/delete | CategoryController.DeleteAssetCategory | axionpro.api/Controllers/Asset/CategoryController.cs:99 |
| Used by Angular | GET /api/Asset/Category/get | CategoryController.GetAllAssetCategory | axionpro.api/Controllers/Asset/CategoryController.cs:46 |
| Used by Angular | PUT /api/Asset/Category/update | CategoryController.UpdateAssetCategory | axionpro.api/Controllers/Asset/CategoryController.cs:81 |
| Used by Angular | DELETE /api/Asset/delete | AssetController.DeleteAsset | axionpro.api/Controllers/Asset/AssetController.cs:98 |
| Used by Angular | GET /api/Asset/get | AssetController.GetAllAssets | axionpro.api/Controllers/Asset/AssetController.cs:49 |
| Used by Angular | POST /api/Asset/Status/add | StatusController.AddAssetStatus | axionpro.api/Controllers/Asset/StatusController.cs:62 |
| Used by Angular | DELETE /api/Asset/Status/delete | StatusController.DeleteAssetStatus | axionpro.api/Controllers/Asset/StatusController.cs:97 |
| Used by Angular | GET /api/Asset/Status/get | StatusController.GetByIdAssetStatus | axionpro.api/Controllers/Asset/StatusController.cs:46 |
| Used by Angular | PUT /api/Asset/Status/update | StatusController.UpdateAssetStatus | axionpro.api/Controllers/Asset/StatusController.cs:79 |
| Used by Angular | POST /api/Asset/Type/add | TypeController.AddAssetType | axionpro.api/Controllers/Asset/TypeController.cs:64 |
| Used by Angular | DELETE /api/Asset/Type/delete | TypeController.DeleteAssetType | axionpro.api/Controllers/Asset/TypeController.cs:104 |
| Used by Angular | GET /api/Asset/Type/get | TypeController.GetAllAssetType | axionpro.api/Controllers/Asset/TypeController.cs:46 |
| Used by Angular | PUT /api/Asset/Type/update | TypeController.UpdateAssetType | axionpro.api/Controllers/Asset/TypeController.cs:86 |
| Used by Angular | PUT /api/Asset/update | AssetController.UpdateAsset | axionpro.api/Controllers/Asset/AssetController.cs:81 |
| No exact Angular call | POST /api/Attendance/mark-attendance | AttendanceController.MarkAttendance | axionpro.api/Controllers/Attendance/AttendanceController.cs:45 |
| No exact Angular call | POST /api/Attendance/timmy-test | AttendanceController.TimmyTest | axionpro.api/Controllers/Attendance/AttendanceController.cs:68 |
| Used by Angular | POST /api/Auth/create-new-password | AuthController.CreateLoginPassword | axionpro.api/Controllers/Login/AuthController.cs:194 |
| Used by Angular | POST /api/Auth/forgot-password | AuthController.EnterLoginId | axionpro.api/Controllers/Login/AuthController.cs:232 |
| No exact Angular call | POST /api/Auth/login | AuthController.Login | axionpro.api/Controllers/Login/AuthController.cs:48 |
| Used by Angular | POST /api/Auth/refresh-token | AuthController.RefreshToken | axionpro.api/Controllers/Login/AuthController.cs:68 |
| Used by Angular | POST /api/Auth/resend-credential | AuthController.CreateNewLoginPasswordURL | axionpro.api/Controllers/Login/AuthController.cs:173 |
| Used by Angular | POST /api/Auth/update-login-password | AuthController.SetLoginPassword | axionpro.api/Controllers/Login/AuthController.cs:153 |
| Used by Angular | POST /api/Auth/validate-forgot-password-otp | AuthController.ValidateForgotPasswordOtp | axionpro.api/Controllers/Login/AuthController.cs:274 |
| No exact Angular call | POST /api/Category/get | CategoryController.GetAllMainCategories | axionpro.api/Controllers/Category/CategoryController.cs:39 |
| No exact Angular call | POST /api/Category/getallmainchildcategory | CategoryController.GetAllMainChildCategories | axionpro.api/Controllers/Category/CategoryController.cs:73 |
| No exact Angular call | POST /api/Client/add | ClientController.CreateClientType | axionpro.api/Controllers/Client/ClientController.cs:90 |
| No exact Angular call | GET /api/Client/get | ClientController.GetAllClientType | axionpro.api/Controllers/Client/ClientController.cs:41 |
| No exact Angular call | POST /api/Client/update | ClientController.UpdateClientType | axionpro.api/Controllers/Client/ClientController.cs:116 |
| Used by Angular | GET /api/ClientInfo/detect-device | ClientInfoController.GetDeviceInfo | axionpro.api/Controllers/ClientInfo/ClientInfoController.cs:38 |
| Used by Angular | GET /api/CommonMenu | CommonMenuController.Get | axionpro.api/Controllers/CommonMenu/CommonMenuController.cs:50 |
| No exact Angular call | POST /api/CommonModule/add | CommonModuleController.AddModule | axionpro.api/Controllers/Module/CommonModuleController.cs:45 |
| No exact Angular call | GET /api/Company/{firstname}/{lastname} | CompanyController.Get | axionpro.api/Controllers/Company/CompanyController.cs:45 |
| No exact Angular call | POST /api/ComplianceRule/update | ComplianceRuleController.UpdateComplianceRuleyAsync | axionpro.api/Controllers/Compliance/ComplianceRuleController.cs:40 |
| Used by Angular | POST /api/Department/add | DepartmentController.CreateDepartmentAsync | axionpro.api/Controllers/Department/DepartmentController.cs:66 |
| Used by Angular | DELETE /api/Department/delete | DepartmentController.DeleteDepartmentAsync | axionpro.api/Controllers/Department/DepartmentController.cs:138 |
| Used by Angular | GET /api/Department/get | DepartmentController.GetAllDepartmentsAsync | axionpro.api/Controllers/Department/DepartmentController.cs:42 |
| Used by Angular | GET /api/Department/option | DepartmentController.getDepartment | axionpro.api/Controllers/Department/DepartmentController.cs:117 |
| Used by Angular | PUT /api/Department/update | DepartmentController.UpdateDepartmentAsync | axionpro.api/Controllers/Department/DepartmentController.cs:93 |
| Used by Angular | POST /api/Designation/add | DesignationController.CreateDesignation | axionpro.api/Controllers/Designation/DesignationController.cs:103 |
| Used by Angular | DELETE /api/Designation/delete | DesignationController.Delete | axionpro.api/Controllers/Designation/DesignationController.cs:128 |
| No exact Angular call | POST /api/Designation/Department/Group/get | DesignationController.GetAllDepartmentAsyc | axionpro.api/Controllers/Designation/DesignationController.cs:63 |
| Used by Angular | GET /api/Designation/get | DesignationController.GetAllDesignationAsyc | axionpro.api/Controllers/Designation/DesignationController.cs:45 |
| Used by Angular | GET /api/Designation/option | DesignationController.getDesignation | axionpro.api/Controllers/Designation/DesignationController.cs:84 |
| Used by Angular | PUT /api/Designation/update | DesignationController.UpdateDesignation | axionpro.api/Controllers/Designation/DesignationController.cs:152 |
| Used by Angular | POST /api/DeviceMaster/create | DeviceMasterController.Create | axionpro.api/Controllers/HostDevice/DeviceMasterController.cs:32 |
| Used by Angular | DELETE /api/DeviceMaster/delete/{id:long} | DeviceMasterController.Delete | axionpro.api/Controllers/HostDevice/DeviceMasterController.cs:97 |
| Used by Angular | GET /api/DeviceMaster/get-all | DeviceMasterController.GetAll | axionpro.api/Controllers/HostDevice/DeviceMasterController.cs:58 |
| Used by Angular | GET /api/DeviceMaster/get-by-id/{id:long} | DeviceMasterController.GetById | axionpro.api/Controllers/HostDevice/DeviceMasterController.cs:45 |
| Used by Angular | POST /api/DeviceMaster/update | DeviceMasterController.Update | axionpro.api/Controllers/HostDevice/DeviceMasterController.cs:71 |
| Used by Angular | POST /api/DeviceMaster/update-status | DeviceMasterController.UpdateStatus | axionpro.api/Controllers/HostDevice/DeviceMasterController.cs:84 |
| No exact Angular call | GET /api/EmailTemplate/get-template-by-code | EmailTemplateController.GetTemplateByCodeAsync | axionpro.api/Controllers/EmailTemplate/EmailTemplateController.cs:43 |
| No exact Angular call | POST /api/EmailTemplate/send-template | EmailTemplateController.SendTemplatedEmail | axionpro.api/Controllers/EmailTemplate/EmailTemplateController.cs:61 |
| Used by Angular | POST /api/Employee/Bank/create | BankController.CreateBankInfo | axionpro.api/Controllers/Employee/BankController.cs:49 |
| Used by Angular | DELETE /api/Employee/Bank/delete | BankController.Delete | axionpro.api/Controllers/Employee/BankController.cs:99 |
| Used by Angular | GET /api/Employee/Bank/get | BankController.GetBankinfo | axionpro.api/Controllers/Employee/BankController.cs:77 |
| Used by Angular | POST /api/Employee/Bank/update | BankController.Update | axionpro.api/Controllers/Employee/BankController.cs:120 |
| Used by Angular | POST /api/Employee/Contact/create | ContactController.CreateContactInfo | axionpro.api/Controllers/Employee/ContactController.cs:45 |
| Used by Angular | DELETE /api/Employee/Contact/delete | ContactController.Delete | axionpro.api/Controllers/Employee/ContactController.cs:115 |
| Used by Angular | GET /api/Employee/Contact/get | ContactController.GetBankinfo | axionpro.api/Controllers/Employee/ContactController.cs:73 |
| Used by Angular | POST /api/Employee/Contact/update | ContactController.UpdateContact | axionpro.api/Controllers/Employee/ContactController.cs:92 |
| Used by Angular | POST /api/Employee/create | EmployeeController.CreateEmployee | axionpro.api/Controllers/Employee/EmployeeController.cs:51 |
| Used by Angular | DELETE /api/Employee/delete-all | EmployeeController.Delete | axionpro.api/Controllers/Employee/EmployeeController.cs:317 |
| Used by Angular | POST /api/Employee/Dependent/create | DependentController.CreateDependentInfo | axionpro.api/Controllers/Employee/DependentController.cs:47 |
| Used by Angular | DELETE /api/Employee/Dependent/delete | DependentController.Delete | axionpro.api/Controllers/Employee/DependentController.cs:114 |
| Used by Angular | GET /api/Employee/Dependent/get | DependentController.Getinfo | axionpro.api/Controllers/Employee/DependentController.cs:74 |
| Used by Angular | GET /api/Employee/Dependent/get-in-detail | DependentController.GetInDetail | axionpro.api/Controllers/Employee/DependentController.cs:94 |
| Used by Angular | POST /api/Employee/Dependent/update | DependentController.Update | axionpro.api/Controllers/Employee/DependentController.cs:138 |
| Used by Angular | POST /api/Employee/Education/create | EducationController.CreateEmployee | axionpro.api/Controllers/Employee/EducationController.cs:40 |
| Used by Angular | DELETE /api/Employee/Education/delete | EducationController.Delete | axionpro.api/Controllers/Employee/EducationController.cs:77 |
| Used by Angular | GET /api/Employee/Education/get | EducationController.GetAllEmployeeInfo | axionpro.api/Controllers/Employee/EducationController.cs:59 |
| Used by Angular | POST /api/Employee/Education/update-education | EducationController.UpdateEducation | axionpro.api/Controllers/Employee/EducationController.cs:99 |
| Used by Angular | POST /api/Employee/Experience/create | ExperienceController.CreateExperience | axionpro.api/Controllers/Employee/ExperienceController.cs:47 |
| Used by Angular | DELETE /api/Employee/Experience/delete | ExperienceController.Delete | axionpro.api/Controllers/Employee/ExperienceController.cs:104 |
| Used by Angular | DELETE /api/Employee/Experience/delete-doc | ExperienceController.DeleteDoc | axionpro.api/Controllers/Employee/ExperienceController.cs:126 |
| Used by Angular | GET /api/Employee/Experience/get | ExperienceController.GetAllexperinceInfo | axionpro.api/Controllers/Employee/ExperienceController.cs:64 |
| Used by Angular | POST /api/Employee/Experience/update | ExperienceController.Update | axionpro.api/Controllers/Employee/ExperienceController.cs:85 |
| Used by Angular | GET /api/Employee/get | EmployeeController.GetEmployee | axionpro.api/Controllers/Employee/EmployeeController.cs:218 |
| Used by Angular | GET /api/Employee/get-all | EmployeeController.GetAllEmployee | axionpro.api/Controllers/Employee/EmployeeController.cs:295 |
| Used by Angular | GET /api/Employee/get-all-percentage | EmployeeController.GetAllEmployeePercentageAsync | axionpro.api/Controllers/Employee/EmployeeController.cs:186 |
| Used by Angular | GET /api/Employee/get-profile-summary | EmployeeController.GetEmployeeProfileSummary | axionpro.api/Controllers/Employee/EmployeeController.cs:271 |
| Used by Angular | GET /api/Employee/get-summary | EmployeeController.GetEmployeeSummary | axionpro.api/Controllers/Employee/EmployeeController.cs:242 |
| Used by Angular | GET /api/Employee/Image/get | EmployeeController.GetAllEmployeeImage | axionpro.api/Controllers/Employee/EmployeeController.cs:106 |
| Used by Angular | DELETE /api/Employee/Insurance/delete | InsuranceController.DeleteEnrolledEmployee | axionpro.api/Controllers/Employee/InsuranceController.cs:65 |
| Used by Angular | POST /api/Employee/Insurance/employee-insurance-enroll | InsuranceController.EnrolledEmployee | axionpro.api/Controllers/Employee/InsuranceController.cs:47 |
| Used by Angular | GET /api/Employee/Insurance/get-all-enroll | InsuranceController.Get | axionpro.api/Controllers/Employee/InsuranceController.cs:83 |
| Used by Angular | POST /api/Employee/official/update | EmployeeController.OfficialUpdate | axionpro.api/Controllers/Employee/EmployeeController.cs:396 |
| Used by Angular | POST /api/Employee/profile/pic/update | EmployeeController.UpdateProfieImage | axionpro.api/Controllers/Employee/EmployeeController.cs:84 |
| Used by Angular | POST /api/Employee/Sensitive/Create | SensitiveController.Createpersonalinfo | axionpro.api/Controllers/Employee/SensitiveController.cs:44 |
| Used by Angular | GET /api/Employee/Sensitive/get | SensitiveController.GetSensitiveData | axionpro.api/Controllers/Employee/SensitiveController.cs:63 |
| Used by Angular | POST /api/Employee/update | EmployeeController.Update | axionpro.api/Controllers/Employee/EmployeeController.cs:369 |
| Used by Angular | POST /api/Employee/update-bulk | EmployeeController.UpdateSectionStatusBulk | axionpro.api/Controllers/Employee/EmployeeController.cs:167 |
| Used by Angular | POST /api/Employee/update-edit-status | EmployeeController.UpdateSectionStatusBulk | axionpro.api/Controllers/Employee/EmployeeController.cs:128 |
| Used by Angular | PUT /api/Employee/update-status | EmployeeController.UpdateEmployeeStatus | axionpro.api/Controllers/Employee/EmployeeController.cs:339 |
| Used by Angular | POST /api/Employee/update-verification-status | EmployeeController.UpdateVerificationStatus | axionpro.api/Controllers/Employee/EmployeeController.cs:148 |
| No exact Angular call | POST /api/EmployeeLeavePolicy/add | EmployeeLeavePolicyController.MapEmployeeyAsync | axionpro.api/Controllers/Leave/EmployeeLeavePolicyController.cs:41 |
| No exact Angular call | GET /api/EmployeeLeavePolicy/EmployeeLeavePolicy/Mapped/get | EmployeeLeavePolicyController.GetAllEmployeeLeavePoliciesAsync | axionpro.api/Controllers/Leave/EmployeeLeavePolicyController.cs:111 |
| No exact Angular call | POST /api/EmployeeLeavePolicy/LeaveBalance/update | EmployeeLeavePolicyController.UpdateEmployeeyAsync | axionpro.api/Controllers/Leave/EmployeeLeavePolicyController.cs:59 |
| No exact Angular call | POST /api/EmployeeLeavePolicy/map | EmployeeLeavePolicyController.MapEmployeeyAsync | axionpro.api/Controllers/Leave/EmployeeLeavePolicyController.cs:77 |
| No exact Angular call | GET /api/EmployeeLeavePolicy/Mapped/Leave/Policy/get | EmployeeLeavePolicyController.GetAllLeavePoliciesAsync | axionpro.api/Controllers/Leave/EmployeeLeavePolicyController.cs:94 |
| No exact Angular call | POST /api/EmployeeLeavePolicy/update | EmployeeLeavePolicyController.UpdateLeavePolicyAsync | axionpro.api/Controllers/Leave/EmployeeLeavePolicyController.cs:127 |
| Used by Angular | GET /api/EmployeeType/get | EmployeeTypeController.GetAllEmployeeType | axionpro.api/Controllers/EmployeeType/EmployeeTypeController.cs:56 |
| Used by Angular | GET /api/EmployeeType/option | EmployeeTypeController.GetAllEmployeeType | axionpro.api/Controllers/EmployeeType/EmployeeTypeController.cs:106 |
| No exact Angular call | GET /api/Entity/get | EntityController.GetStaticEntityNames | axionpro.api/Controllers/Entity/EntityController.cs:37 |
| No exact Angular call | GET /api/Enum/get-all-currencies | EnumController.GetCurrencies | axionpro.api/Controllers/EnumTypes/EnumController.cs:40 |
| No exact Angular call | POST /api/FileUpload/UploadAsset/upload | FileUploadController.UploadAsset | axionpro.api/Controllers/FileUpload/FileUploadController.cs:42 |
| No exact Angular call | GET /api/Gender/get | GenderController.GetAllGenderAsync | axionpro.api/Controllers/Gender/GenderController.cs:63 |
| Used by Angular | GET /api/Gender/option | GenderController.getGender | axionpro.api/Controllers/Gender/GenderController.cs:44 |
| No exact Angular call | GET /api/HolidayCalandar/get | HolidayCalandarController.GetAllEmployeeInfo | axionpro.api/Controllers/HolidayCalandar/HolidayCalandarController.cs:42 |
| Used by Angular | POST /api/Host/change-host-user-password | HostController.ChangeHostUserPassword | axionpro.api/Controllers/Host/HostController.cs:177 |
| Used by Angular | POST /api/Host/create-host-role | HostController.CreateHostRole | axionpro.api/Controllers/Host/HostController.cs:64 |
| Used by Angular | POST /api/Host/create-host-user | HostController.CreateHostUser | axionpro.api/Controllers/Host/HostController.cs:46 |
| Used by Angular | POST /api/Host/delete-host-role | HostController.DeleteHostRole | axionpro.api/Controllers/Host/HostController.cs:282 |
| Used by Angular | POST /api/Host/delete-host-user | HostController.DeleteHostUser | axionpro.api/Controllers/Host/HostController.cs:152 |
| Used by Angular | GET /api/Host/get-all-host-roles | HostController.GetAllHostRoles | axionpro.api/Controllers/Host/HostController.cs:242 |
| Used by Angular | GET /api/Host/get-all-host-users | HostController.GetAllHostUsers | axionpro.api/Controllers/Host/HostController.cs:109 |
| No exact Angular call | GET /api/Host/get-host-module-by-id/{id:int} | HostController.GetHostModuleById | axionpro.api/Controllers/Host/HostController.cs:332 |
| Used by Angular | GET /api/Host/get-host-modules | HostController.GetHostModules | axionpro.api/Controllers/Host/HostController.cs:309 |
| Used by Angular | GET /api/Host/get-host-role-by-id/{id} | HostController.GetHostRoleById | axionpro.api/Controllers/Host/HostController.cs:223 |
| Used by Angular | GET /api/Host/get-host-user-by-id/{id} | HostController.GetHostUserById | axionpro.api/Controllers/Host/HostController.cs:87 |
| Used by Angular | POST /api/Host/reset-host-user-password | HostController.ResetHostUserPassword | axionpro.api/Controllers/Host/HostController.cs:198 |
| Used by Angular | POST /api/Host/update-host-role | HostController.UpdateHostRole | axionpro.api/Controllers/Host/HostController.cs:261 |
| Used by Angular | POST /api/Host/update-host-user | HostController.UpdateHostUser | axionpro.api/Controllers/Host/HostController.cs:131 |
| No exact Angular call | GET /api/HostAccess/bootstrap | HostAccessController.GetBootstrap | axionpro.api/Controllers/Host/HostAccessController.cs:50 |
| Used by Angular | GET /api/HostRolePermission/get-role-module-permissions/{hostRoleId:long} | HostRolePermissionController.GetRoleModulePermissions | axionpro.api/Controllers/Host/HostRolePermissionController.cs:52 |
| Used by Angular | POST /api/HostRolePermission/save-role-module-permissions | HostRolePermissionController.SaveRoleModulePermissions | axionpro.api/Controllers/Host/HostRolePermissionController.cs:73 |
| Used by Angular | POST /api/Insurance/create | InsuranceController.Create | axionpro.api/Controllers/Insurance/InsuranceController.cs:40 |
| Used by Angular | DELETE /api/Insurance/delete | InsuranceController.Delete | axionpro.api/Controllers/Insurance/InsuranceController.cs:125 |
| Used by Angular | GET /api/Insurance/get-all | InsuranceController.GetList | axionpro.api/Controllers/Insurance/InsuranceController.cs:106 |
| Used by Angular | GET /api/Insurance/get-ddl | InsuranceController.GetList | axionpro.api/Controllers/Insurance/InsuranceController.cs:62 |
| Used by Angular | GET /api/Insurance/get-detail-ddl | InsuranceController.GetDetailList | axionpro.api/Controllers/Insurance/InsuranceController.cs:84 |
| Used by Angular | PUT /api/Insurance/update | InsuranceController.Update | axionpro.api/Controllers/Insurance/InsuranceController.cs:145 |
| No exact Angular call | POST /api/Leave/add | LeaveController.CreateLeaveType | axionpro.api/Controllers/Leave/LeaveController.cs:40 |
| No exact Angular call | GET /api/Leave/delete | LeaveController.DeleteLeave | axionpro.api/Controllers/Leave/LeaveController.cs:96 |
| No exact Angular call | GET /api/Leave/get | LeaveController.GetAllLeaves | axionpro.api/Controllers/Leave/LeaveController.cs:64 |
| No exact Angular call | POST /api/Leave/update | LeaveController.UpdateLeave | axionpro.api/Controllers/Leave/LeaveController.cs:79 |
| Used by Angular | POST /api/LeaveRule/create | LeaveRuleController.CreateLeaveRuleAsync | axionpro.api/Controllers/Leave/LeaveRuleController.cs:40 |
| Used by Angular | POST /api/LeaveRule/delete | LeaveRuleController.DeleteLeavePolicy | axionpro.api/Controllers/Leave/LeaveRuleController.cs:111 |
| Used by Angular | GET /api/LeaveRule/get | LeaveRuleController.GetAllLeaveRuleAsync | axionpro.api/Controllers/Leave/LeaveRuleController.cs:57 |
| No exact Angular call | GET /api/LeaveRule/LeaveRule/Sandwich/get | LeaveRuleController.GetAllLeaveRuleSandwichAsync | axionpro.api/Controllers/Leave/LeaveRuleController.cs:75 |
| Used by Angular | POST /api/LeaveRule/update | LeaveRuleController.UpdateLeavePolicyAsync | axionpro.api/Controllers/Leave/LeaveRuleController.cs:93 |
| Used by Angular | GET /api/Location/country/option | LocationController.getCountry | axionpro.api/Controllers/Location/LocationController.cs:54 |
| Used by Angular | GET /api/Location/District/option | LocationController.getDistrict | axionpro.api/Controllers/Location/LocationController.cs:86 |
| Used by Angular | GET /api/Location/State/option | LocationController.getState | axionpro.api/Controllers/Location/LocationController.cs:70 |
| No exact Angular call | POST /api/MenuStructure/get-menus-structure | MenuStructureController.GetAllMenuStructure | axionpro.api/Controllers/MenuStructureView/MenuStructureController.cs:31 |
| Used by Angular | POST /api/ModuleOperation/create | ModuleOperationController.CreateModuleOperation | axionpro.api/Controllers/Module/ModuleOperationController.cs:50 |
| Used by Angular | DELETE /api/ModuleOperation/delete/{id:int} | ModuleOperationController.DeleteModuleOperation | axionpro.api/Controllers/Module/ModuleOperationController.cs:90 |
| Used by Angular | GET /api/ModuleOperation/get-all | ModuleOperationController.GetAllModuleOperations | axionpro.api/Controllers/Module/ModuleOperationController.cs:129 |
| Used by Angular | GET /api/ModuleOperation/get-by-id/{id:int} | ModuleOperationController.GetModuleOperationById | axionpro.api/Controllers/Module/ModuleOperationController.cs:110 |
| Used by Angular | POST /api/ModuleOperation/update | ModuleOperationController.UpdateModuleOperation | axionpro.api/Controllers/Module/ModuleOperationController.cs:70 |
| Used by Angular | POST /api/NewLogin/login | NewLoginController.Login | axionpro.api/Controllers/Login/NewLoginController.cs:49 |
| Used by Angular | POST /api/OperationsMaster/create-operation | OperationsMasterController.CreateOperation | axionpro.api/Controllers/OperationsMaster/OperationsMasterController.cs:49 |
| Used by Angular | DELETE /api/OperationsMaster/delete-operation/{operationId:int} | OperationsMasterController.DeleteOperation | axionpro.api/Controllers/OperationsMaster/OperationsMasterController.cs:97 |
| Used by Angular | GET /api/OperationsMaster/get-all-operations | OperationsMasterController.GetAllOperations | axionpro.api/Controllers/OperationsMaster/OperationsMasterController.cs:144 |
| Used by Angular | GET /api/OperationsMaster/get-operation/{operationId:int} | OperationsMasterController.GetOperationById | axionpro.api/Controllers/OperationsMaster/OperationsMasterController.cs:121 |
| Used by Angular | POST /api/OperationsMaster/update-operation | OperationsMasterController.UpdateOperation | axionpro.api/Controllers/OperationsMaster/OperationsMasterController.cs:73 |
| Used by Angular | POST /api/Option/create | OptionController.CreateOperation | axionpro.api/Controllers/Operation/OperationController.cs:64 |
| Used by Angular | GET /api/Option/get | OptionController.GetAllOperationAsyc | axionpro.api/Controllers/Operation/OperationController.cs:46 |
| Used by Angular | GET /api/Option/has-access | OptionController.HasPageOperationAccess | axionpro.api/Controllers/Operation/OperationController.cs:104 |
| Used by Angular | POST /api/Option/update | OptionController.UpdateOperation | axionpro.api/Controllers/Operation/OperationController.cs:85 |
| Used by Angular | GET /api/ParentModule/{id:int} | ParentModuleController.GetModuleById | axionpro.api/Controllers/Module/ParentModuleController.cs:139 |
| Used by Angular | PUT /api/ParentModule/{id:int} | ParentModuleController.UpdateModule | axionpro.api/Controllers/Module/ParentModuleController.cs:79 |
| Used by Angular | PATCH /api/ParentModule/{id:int}/status | ParentModuleController.UpdateModuleStatus | axionpro.api/Controllers/Module/ParentModuleController.cs:108 |
| Used by Angular | POST /api/ParentModule/add | ParentModuleController.AddModule | axionpro.api/Controllers/Module/ParentModuleController.cs:53 |
| Used by Angular | GET /api/ParentModule/get-module-headers | ParentModuleController.GetModuleHeaders | axionpro.api/Controllers/Module/ParentModuleController.cs:167 |
| No exact Angular call | GET /api/PlanModuleMapping/options/{subscriptionPlanId:int} | PlanModuleMappingController.GetOptions | axionpro.api/Controllers/Subscription/PlanModuleMappingController.cs:49 |
| Used by Angular | POST /api/PlanModuleMapping/save | PlanModuleMappingController.Save | axionpro.api/Controllers/Subscription/PlanModuleMappingController.cs:69 |
| Used by Angular | POST /api/PolicyMappingLeaveType/delete | PolicyMappingLeaveTypeController.DeleteLeavePolicy | axionpro.api/Controllers/Leave/PolicyMappingLeaveTypeController.cs:107 |
| Used by Angular | GET /api/PolicyMappingLeaveType/get | PolicyMappingLeaveTypeController.GetAllLeavePoliciesAsync | axionpro.api/Controllers/Leave/PolicyMappingLeaveTypeController.cs:58 |
| No exact Angular call | GET /api/PolicyMappingLeaveType/LeavePolicy/EmployeeType/get | PolicyMappingLeaveTypeController.GetAllLeavePoliciesByEmployeeIdAsync | axionpro.api/Controllers/Leave/PolicyMappingLeaveTypeController.cs:75 |
| Used by Angular | POST /api/PolicyMappingLeaveType/map | PolicyMappingLeaveTypeController.CreateLeavePolicyAsync | axionpro.api/Controllers/Leave/PolicyMappingLeaveTypeController.cs:41 |
| Used by Angular | POST /api/PolicyMappingLeaveType/update | PolicyMappingLeaveTypeController.UpdateLeavePolicyAsync | axionpro.api/Controllers/Leave/PolicyMappingLeaveTypeController.cs:91 |
| Used by Angular | POST /api/PolicyType/create | PolicyTypeController.CreatePolicyTypeAsync | axionpro.api/Controllers/Policies/PolicyTypeController.cs:119 |
| Used by Angular | DELETE /api/PolicyType/delete | PolicyTypeController.DeletePolicyTypeAsync | axionpro.api/Controllers/Policies/PolicyTypeController.cs:154 |
| No exact Angular call | DELETE /api/PolicyType/delete-doc | PolicyTypeController.DeletePolicyTypeDocOnlyAsync | axionpro.api/Controllers/Policies/PolicyTypeController.cs:170 |
| Used by Angular | GET /api/PolicyType/get-all | PolicyTypeController.GetAllPolicyTypesAsync | axionpro.api/Controllers/Policies/PolicyTypeController.cs:44 |
| Used by Angular | GET /api/PolicyType/get-all-unstruct | PolicyTypeController.GetUnstructuredPolicyTypesAsync | axionpro.api/Controllers/Policies/PolicyTypeController.cs:91 |
| Used by Angular | GET /api/PolicyType/get-ddl | PolicyTypeController.GetDDLPolicyTypesAsync | axionpro.api/Controllers/Policies/PolicyTypeController.cs:63 |
| Used by Angular | POST /api/PolicyType/update | PolicyTypeController.UpdatePolicyTypeAsync | axionpro.api/Controllers/Policies/PolicyTypeController.cs:136 |
| Used by Angular | DELETE /api/PolicyTypeInsuranceMap/delete | PolicyTypeInsuranceMapController.Delete | axionpro.api/Controllers/Insurance/PolicyTypeInsuranceMapController.cs:122 |
| Used by Angular | GET /api/PolicyTypeInsuranceMap/get-all | PolicyTypeInsuranceMapController.GetList | axionpro.api/Controllers/Insurance/PolicyTypeInsuranceMapController.cs:84 |
| No exact Angular call | GET /api/PolicyTypeInsuranceMap/get-all-map-insurance | PolicyTypeInsuranceMapController.GetList | axionpro.api/Controllers/Insurance/PolicyTypeInsuranceMapController.cs:64 |
| Used by Angular | GET /api/PolicyTypeInsuranceMap/get-details | PolicyTypeInsuranceMapController.GetDetailList | axionpro.api/Controllers/Insurance/PolicyTypeInsuranceMapController.cs:104 |
| Used by Angular | POST /api/PolicyTypeInsuranceMap/map | PolicyTypeInsuranceMapController.Create | axionpro.api/Controllers/Insurance/PolicyTypeInsuranceMapController.cs:41 |
| Used by Angular | PUT /api/PolicyTypeInsuranceMap/update | PolicyTypeInsuranceMapController.Update | axionpro.api/Controllers/Insurance/PolicyTypeInsuranceMapController.cs:139 |
| No exact Angular call | POST /api/Registration/AccessDetails | RegistrationController.UserAccessDetailsAsync | axionpro.api/Controllers/Registration/RegistrationController.cs:60 |
| No exact Angular call | POST /api/Registration/candidate | RegistrationController.Login | axionpro.api/Controllers/Registration/RegistrationController.cs:41 |
| Used by Angular | POST /api/ReportingType/create | ReportingTypeController.CreateReportingType | axionpro.api/Controllers/ReportingType/ReportingTypeController.cs:54 |
| Used by Angular | DELETE /api/ReportingType/delete | ReportingTypeController.DeleteReportingType | axionpro.api/Controllers/ReportingType/ReportingTypeController.cs:146 |
| Used by Angular | GET /api/ReportingType/get-all | ReportingTypeController.GetAllReportingTypes | axionpro.api/Controllers/ReportingType/ReportingTypeController.cs:79 |
| No exact Angular call | GET /api/ReportingType/get-by-id | ReportingTypeController.GetReportingTypeById | axionpro.api/Controllers/ReportingType/ReportingTypeController.cs:101 |
| Used by Angular | PUT /api/ReportingType/update | ReportingTypeController.UpdateReportingType | axionpro.api/Controllers/ReportingType/ReportingTypeController.cs:123 |
| Used by Angular | POST /api/Role/add | RoleController.CreateRole | axionpro.api/Controllers/Role/RoleController.cs:83 |
| Used by Angular | DELETE /api/Role/delete | RoleController.DeleteRole | axionpro.api/Controllers/Role/RoleController.cs:117 |
| Used by Angular | GET /api/Role/get | RoleController.GetAllRoles | axionpro.api/Controllers/Role/RoleController.cs:100 |
| Used by Angular | GET /api/Role/option | RoleController.getRole | axionpro.api/Controllers/Role/RoleController.cs:65 |
| Used by Angular | PUT /api/Role/update | RoleController.UpdateRole | axionpro.api/Controllers/Role/RoleController.cs:48 |
| No exact Angular call | POST /api/Sandwich/add | RuleController.CreateSandwichRule | axionpro.api/Controllers/SandwichRule/RuleController.cs:148 |
| No exact Angular call | POST /api/Sandwich/DayCombination/add | RuleController.GetAllDayCombinationByTenantUser | axionpro.api/Controllers/SandwichRule/RuleController.cs:50 |
| No exact Angular call | POST /api/Sandwich/DayCombination/delete | RuleController.DeleteDayCombinationByTenantUser | axionpro.api/Controllers/SandwichRule/RuleController.cs:89 |
| No exact Angular call | POST /api/Sandwich/DayCombination/get | RuleController.GetAllDayCombinationByTenantUser | axionpro.api/Controllers/SandwichRule/RuleController.cs:109 |
| No exact Angular call | POST /api/Sandwich/DayCombination/update | RuleController.UpdateDayCombinationByTenantUser | axionpro.api/Controllers/SandwichRule/RuleController.cs:70 |
| No exact Angular call | DELETE /api/Sandwich/delete | RuleController.DeleteSandwichRule | axionpro.api/Controllers/SandwichRule/RuleController.cs:185 |
| No exact Angular call | GET /api/Sandwich/get | RuleController.GetAllSandwichRule | axionpro.api/Controllers/SandwichRule/RuleController.cs:130 |
| No exact Angular call | POST /api/Sandwich/update | RuleController.UpdateSandwichRule | axionpro.api/Controllers/SandwichRule/RuleController.cs:165 |
| Used by Angular | GET /api/StatData/Dashboard/Employees/Statistics | StatDataController.GetEmployeeDashboardSummaryAsync | axionpro.api/Controllers/Stats/StatDataController.cs:46 |
| Used by Angular | GET /api/StatData/Manager/Statistic/Asset | StatDataController.Asset | axionpro.api/Controllers/Stats/StatDataController.cs:86 |
| Used by Angular | GET /api/StatData/Manager/Statistics/Dashboard/get | StatDataController.dashboard | axionpro.api/Controllers/Stats/StatDataController.cs:64 |
| Used by Angular | GET /api/SubModule/{id:int} | SubModuleController.GetModuleById | axionpro.api/Controllers/Module/SubModuleController.cs:93 |
| Used by Angular | PUT /api/SubModule/{id:int} | SubModuleController.UpdateModule | axionpro.api/Controllers/Module/SubModuleController.cs:71 |
| Used by Angular | PATCH /api/SubModule/{id:int}/status | SubModuleController.UpdateModuleStatus | axionpro.api/Controllers/Module/SubModuleController.cs:163 |
| Used by Angular | POST /api/SubModule/add | SubModuleController.AddModule | axionpro.api/Controllers/Module/SubModuleController.cs:50 |
| Used by Angular | GET /api/SubModule/list | SubModuleController.GetModules | axionpro.api/Controllers/Module/SubModuleController.cs:116 |
| No exact Angular call | GET /api/SubModule/parent/{parentModuleId:int} | SubModuleController.GetModulesByParent | axionpro.api/Controllers/Module/SubModuleController.cs:140 |
| Used by Angular | PUT /api/Subscription/{id:long} | SubscriptionController.UpdateSubscription | axionpro.api/Controllers/Subscription/SubscriptionController.cs:165 |
| No exact Angular call | POST /api/Subscription/add | SubscriptionController.CreateSubscription | axionpro.api/Controllers/Subscription/SubscriptionController.cs:142 |
| No exact Angular call | POST /api/Subscription/delete-subscription-plan | SubscriptionController.DeleteSubscriptionPlan | axionpro.api/Controllers/Subscription/SubscriptionController.cs:188 |
| No exact Angular call | POST /api/Subscription/get-all-host-subscription-plans | SubscriptionController.GetAllHostSubscriptionPlans | axionpro.api/Controllers/Subscription/SubscriptionController.cs:76 |
| No exact Angular call | GET /api/Subscription/get-all-subscription-plan | SubscriptionController.GetAllSubscriptionPlan | axionpro.api/Controllers/Subscription/SubscriptionController.cs:54 |
| No exact Angular call | GET /api/Subscription/get-all-tenant-accessible-modules | SubscriptionController.GetAllTenantAccessibleModules | axionpro.api/Controllers/Subscription/SubscriptionController.cs:116 |
| No exact Angular call | GET /api/Subscription/get-tenant-subscription-plan-info | SubscriptionController.GetTenantSubscriptionPlanInfo | axionpro.api/Controllers/Subscription/SubscriptionController.cs:96 |
| No exact Angular call | DELETE /api/Tenant/{id} | TenantController.DeleteHostManagedTenantAsync | axionpro.api/Controllers/Tenant/TenantController.cs:168 |
| No exact Angular call | PUT /api/Tenant/{id} | TenantController.UpdateHostManagedTenantAsync | axionpro.api/Controllers/Tenant/TenantController.cs:138 |
| Used by Angular | POST /api/Tenant/{id}/resend-verification | TenantController.ResendTenantVerificationAsync | axionpro.api/Controllers/Tenant/TenantController.cs:197 |
| Used by Angular | POST /api/Tenant/activate-tenant | TenantController.ActivateTenantAsync | axionpro.api/Controllers/Tenant/TenantController.cs:247 |
| No exact Angular call | POST /api/Tenant/create-host-user | TenantController.CreateHostUser | axionpro.api/Controllers/Tenant/TenantController.cs:315 |
| Used by Angular | POST /api/Tenant/create-tenant | TenantController.TenantCreation | axionpro.api/Controllers/Tenant/TenantController.cs:67 |
| Used by Angular | POST /api/Tenant/deactivate-tenant | TenantController.DeactivateTenantAsync | axionpro.api/Controllers/Tenant/TenantController.cs:268 |
| Used by Angular | POST /api/Tenant/delete-tenant | TenantController.DeleteTenantAsync | axionpro.api/Controllers/Tenant/TenantController.cs:289 |
| No exact Angular call | POST /api/Tenant/get | TenantController.GetAllTenantEnabledModuleOperationsByTenantIdAsync | axionpro.api/Controllers/Tenant/TenantController.cs:376 |
| No exact Angular call | GET /api/Tenant/get-all-tenant-by-subscription-plan-Id | TenantController.GetAllTenantBySubscriptionIdAsync | axionpro.api/Controllers/Tenant/TenantController.cs:336 |
| Used by Angular | GET /api/Tenant/get-all-tenant-operations | TenantController.GetAllNodeLeafeWithOperationsAsync | axionpro.api/Controllers/Tenant/TenantController.cs:393 |
| Used by Angular | GET /api/Tenant/get-all-tenants | TenantController.GetAllTenantsAsync | axionpro.api/Controllers/Tenant/TenantController.cs:90 |
| Used by Angular | GET /api/Tenant/get-employee-code-pattern | TenantController.GetEmployeeCodePatternAsync | axionpro.api/Controllers/Tenant/TenantController.cs:354 |
| Used by Angular | GET /api/Tenant/get-tenant-by-id | TenantController.GetTenantByIdAsync | axionpro.api/Controllers/Tenant/TenantController.cs:110 |
| Used by Angular | POST /api/Tenant/update-modules-and-operations | TenantController.TenantModuleOperationsUpdate | axionpro.api/Controllers/Tenant/TenantController.cs:415 |
| Used by Angular | POST /api/Tenant/update-tenant | TenantController.UpdateTenantAsync | axionpro.api/Controllers/Tenant/TenantController.cs:225 |
| Used by Angular | POST /api/Tenant/verify | TenantController.VerifyEmail | axionpro.api/Controllers/Tenant/TenantController.cs:436 |
| Used by Angular | POST /api/TenantDevice/create | TenantDeviceController.Create | axionpro.api/Controllers/HostDevice/TenantDeviceController.cs:32 |
| Used by Angular | DELETE /api/TenantDevice/delete/{id:long} | TenantDeviceController.Delete | axionpro.api/Controllers/HostDevice/TenantDeviceController.cs:97 |
| Used by Angular | GET /api/TenantDevice/get-all | TenantDeviceController.GetAll | axionpro.api/Controllers/HostDevice/TenantDeviceController.cs:58 |
| Used by Angular | GET /api/TenantDevice/get-by-id/{id:long} | TenantDeviceController.GetById | axionpro.api/Controllers/HostDevice/TenantDeviceController.cs:45 |
| Used by Angular | POST /api/TenantDevice/update | TenantDeviceController.Update | axionpro.api/Controllers/HostDevice/TenantDeviceController.cs:71 |
| Used by Angular | POST /api/TenantDevice/update-status | TenantDeviceController.UpdateStatus | axionpro.api/Controllers/HostDevice/TenantDeviceController.cs:84 |
| Used by Angular | GET /api/TenantIndustry/get-industries | TenantIndustryController.GetAllTenantBySubscriptionIdAsync | axionpro.api/Controllers/TenantIndustry/TenantIndustryController.cs:44 |
| Used by Angular | GET /api/TenantIndustry/get-tenant-subscription-plan | TenantIndustryController.GetTenantSubscriptionPlanInfoAsync | axionpro.api/Controllers/TenantIndustry/TenantIndustryController.cs:60 |
| No exact Angular call | GET /api/TenantParentModule/{id:int} | TenantParentModuleController.GetModuleById | axionpro.api/Controllers/Module/TenantParentModuleController.cs:105 |
| No exact Angular call | PATCH /api/TenantParentModule/{id:int}/status | TenantParentModuleController.UpdateModuleStatus | axionpro.api/Controllers/Module/TenantParentModuleController.cs:138 |
| No exact Angular call | GET /api/TenantParentModule/get-module-headers | TenantParentModuleController.GetModuleHeaders | axionpro.api/Controllers/Module/TenantParentModuleController.cs:54 |
| No exact Angular call | GET /api/TenantParentModule/list | TenantParentModuleController.GetModules | axionpro.api/Controllers/Module/TenantParentModuleController.cs:80 |
| No exact Angular call | GET /api/TenantUserAccess/bootstrap | TenantUserAccessController.GetBootstrap | axionpro.api/Controllers/TenantUserAccess/TenantUserAccessController.cs:50 |
| Used by Angular | POST /api/Ticket/TicketHeader/create | TicketHeaderController.CreateHeader | axionpro.api/Controllers/Ticket/TicketHeaderController.cs:46 |
| Used by Angular | DELETE /api/Ticket/TicketHeader/delete | TicketHeaderController.DeleteTicketHeader | axionpro.api/Controllers/Ticket/TicketHeaderController.cs:137 |
| Used by Angular | GET /api/Ticket/TicketHeader/get-by-classification-id | TicketHeaderController.GetAllHeaderFilterAsync | axionpro.api/Controllers/Ticket/TicketHeaderController.cs:66 |
| Used by Angular | PUT /api/Ticket/TicketHeader/update | TicketHeaderController.UpdateHeader | axionpro.api/Controllers/Ticket/TicketHeaderController.cs:99 |
| Used by Angular | POST /api/Ticket/TicketType/create | TicketTypeController.CreateTicketType | axionpro.api/Controllers/Ticket/TicketTypeController.cs:49 |
| Used by Angular | GET /api/Ticket/TicketType/ddl-list | TicketTypeController.GetDDLTicketTypes | axionpro.api/Controllers/Ticket/TicketTypeController.cs:88 |
| Used by Angular | DELETE /api/Ticket/TicketType/delete | TicketTypeController.DeleteTicketType | axionpro.api/Controllers/Ticket/TicketTypeController.cs:147 |
| Used by Angular | GET /api/Ticket/TicketType/get-all | TicketTypeController.GetAllTicketTypes | axionpro.api/Controllers/Ticket/TicketTypeController.cs:71 |
| Used by Angular | GET /api/Ticket/TicketType/get-by-header-id | TicketTypeController.GetTicketTypesByHeaderId | axionpro.api/Controllers/Ticket/TicketTypeController.cs:170 |
| No exact Angular call | GET /api/Ticket/TicketType/get-by-id | TicketTypeController.GetTicketTypeById | axionpro.api/Controllers/Ticket/TicketTypeController.cs:107 |
| Used by Angular | PUT /api/Ticket/TicketType/update | TicketTypeController.UpdateTicketType | axionpro.api/Controllers/Ticket/TicketTypeController.cs:127 |
| Used by Angular | GET /api/TicketClassification/all | TicketClassificationController.GetAllTicketClassifications | axionpro.api/Controllers/Ticket/ClassificationController.cs:64 |
| Used by Angular | POST /api/TicketClassification/create | TicketClassificationController.CreateTicketClassification | axionpro.api/Controllers/Ticket/ClassificationController.cs:42 |
| Used by Angular | GET /api/TicketClassification/ddl-list | TicketClassificationController.GetAllTicketClassifications | axionpro.api/Controllers/Ticket/ClassificationController.cs:83 |
| Used by Angular | DELETE /api/TicketClassification/delete | TicketClassificationController.DeleteTicketClassification | axionpro.api/Controllers/Ticket/ClassificationController.cs:147 |
| No exact Angular call | GET /api/TicketClassification/get | TicketClassificationController.GetTicketClassificationById | axionpro.api/Controllers/Ticket/ClassificationController.cs:105 |
| Used by Angular | PUT /api/TicketClassification/update | TicketClassificationController.UpdateTicketClassification | axionpro.api/Controllers/Ticket/ClassificationController.cs:126 |
| No exact Angular call | POST /api/Travel/addtravelmode | TravelController.CreateTravelModeType | axionpro.api/Controllers/Travel/TravelController.cs:59 |
| No exact Angular call | GET /api/Travel/getalltravelmodetype | TravelController.GetAllTravelModeType | axionpro.api/Controllers/Travel/TravelController.cs:42 |
| No exact Angular call | POST /api/Travel/updatetravelmodetype | TravelController.UpdateTravelModeType | axionpro.api/Controllers/Travel/TravelController.cs:76 |
| Used by Angular | POST /api/UserModuleRolePermission/assign-role-permissions | UserModuleRolePermissionController.CreatePermission | axionpro.api/Controllers/UserModuleRolePermission/UserModuleRolePermissionController.cs:37 |
| Used by Angular | GET /api/UserModuleRolePermission/get-role-based-permissions | UserModuleRolePermissionController.GetTenantEnabledOperations | axionpro.api/Controllers/UserModuleRolePermission/UserModuleRolePermissionController.cs:55 |
| Used by Angular | POST /api/UserRole/assign-roles-to-user | UserRoleController.CreatePermission | axionpro.api/Controllers/UserRole/UserRoleController.cs:39 |
| Used by Angular | GET /api/UserRole/get-all-user-roles | UserRoleController.GetTenantEnabledOperations | axionpro.api/Controllers/UserRole/UserRoleController.cs:57 |
| No exact Angular call | POST /api/WorkflowStage/create | WorkflowStageController.CreateWorkflowStage | axionpro.api/Controllers/WorkflowStage/WorkflowStageController.cs:51 |
| No exact Angular call | DELETE /api/WorkflowStage/delete | WorkflowStageController.DeleteWorkflowStage | axionpro.api/Controllers/WorkflowStage/WorkflowStageController.cs:141 |
| No exact Angular call | GET /api/WorkflowStage/get | WorkflowStageController.GetWorkflowStageById | axionpro.api/Controllers/WorkflowStage/WorkflowStageController.cs:97 |
| No exact Angular call | GET /api/WorkflowStage/get-all | WorkflowStageController.GetAllWorkflowStages | axionpro.api/Controllers/WorkflowStage/WorkflowStageController.cs:73 |
| No exact Angular call | PUT /api/WorkflowStage/update | WorkflowStageController.UpdateWorkflowStage | axionpro.api/Controllers/WorkflowStage/WorkflowStageController.cs:120 |

## Local/mock and unresolved HTTP calls

| Classification | Request | Source |
|---|---|---|
| Local/mock HTTP asset | GET 'data/holidays.json' | app/core/services/holidays-api.ts:69 |
| Local/mock HTTP asset | GET 'data/learning/courses.json' | app/core/services/learning-api.ts:29 |
| Local/mock HTTP asset | GET 'data/learning/my-learning.json' | app/core/services/learning-api.ts:45 |
| Local/mock HTTP asset | GET 'data/learning/learning-paths.json' | app/core/services/learning-api.ts:56 |
| Local/mock HTTP asset | GET 'data/learning/assigned.json' | app/core/services/learning-api.ts:62 |
| Local/mock HTTP asset | GET 'data/learning/assessments.json' | app/core/services/learning-api.ts:68 |
| Local/mock HTTP asset | GET 'data/learning/knowledge.json' | app/core/services/learning-api.ts:89 |
| Local/mock HTTP asset | GET 'data/learning/calendar.json' | app/core/services/learning-api.ts:95 |
| Local/mock HTTP asset | GET 'data/learning/certificates.json' | app/core/services/learning-certificate-api.ts:15 |
| Local/mock HTTP asset | GET 'data/learning/dashboard.json' | app/core/services/learning-dashboard-api.ts:15 |
| Local/mock HTTP asset | GET 'data/leaves/my-leaves.json' | app/core/services/leave-api.ts:27 |
| Local/mock HTTP asset | GET 'data/leaves/requested-leaves.json' | app/core/services/leave-api.ts:33 |
| Local/mock HTTP asset | GET 'data/leaves/leave-balances.json' | app/core/services/leave-api.ts:39 |
| Local/mock HTTP asset | GET 'data/leaves/leave-status.json' | app/core/services/leave-api.ts:45 |
| Local/mock HTTP asset | GET 'data/okr.json' | app/core/services/okr-api.ts:34 |
| Local/mock HTTP asset | GET 'data/okr.json' | app/core/services/okr-api.ts:43 |
| Local/mock HTTP asset | GET 'data/okr-stats.json' | app/core/services/okr-api.ts:60 |
| Local/mock HTTP asset | GET 'data/okr-detail-extras.json' | app/core/services/okr-api.ts:71 |
| Local/mock HTTP asset | GET 'data/pms-dashboard.json' | app/core/services/performance-management-api.ts:33 |
| Local/mock HTTP asset | GET 'data/pms-goals.json' | app/core/services/performance-management-api.ts:40 |
| Local/mock HTTP asset | GET 'data/pms-reviews.json' | app/core/services/performance-management-api.ts:46 |
| Local/mock HTTP asset | GET 'data/pms-feedback.json' | app/core/services/performance-management-api.ts:52 |
| Local/mock HTTP asset | GET 'data/pms-one-on-one.json' | app/core/services/performance-management-api.ts:58 |
| Local/mock HTTP asset | GET 'data/pms-history.json' | app/core/services/performance-management-api.ts:64 |
| Local/mock HTTP asset | GET 'data/projects.json' | app/core/services/projects-api.ts:77 |
| Local/mock HTTP asset | GET 'data/project-tasks.json' | app/core/services/projects-api.ts:81 |
| Local/mock HTTP asset | GET 'data/project-schedule.json' | app/core/services/projects-api.ts:87 |
| Local/mock HTTP asset | GET 'data/tasks/tasks.json' | app/core/services/tasks-api.ts:23 |
| Local/mock HTTP asset | GET 'data/tasks/task-meta.json' | app/core/services/tasks-api.ts:39 |
| Local/mock HTTP asset | GET 'data/attendance.json' | app/features/attendance/attendance-api.ts:16 |
| Local/mock HTTP asset | GET 'data/employees-short.json' | app/features/attendance/attendance-api.ts:22 |
| Local/mock HTTP asset | GET 'data/payroll/salary-structures.json' | app/features/payroll/payroll-api.ts:19 |
| Local/mock HTTP asset | GET 'data/payroll/payroll-records.json' | app/features/payroll/payroll-api.ts:25 |
| Local/mock HTTP asset | GET 'data/payroll/payslips.json' | app/features/payroll/payroll-api.ts:31 |
| Local/mock HTTP asset | GET 'data/employees-short.json' | app/features/projects/new-project-dialog/new-project-dialog.ts:135 |
| Local/mock HTTP asset | GET 'data/employees-short.json' | app/features/projects/projects.component.ts:160 |
| Local/mock HTTP asset | GET 'data/employees-short.json' | app/features/tickets/ticket-agents/ticket-agent-add-dialog/ticket-agent-add-dialog.ts:107 |
| Local/mock HTTP asset | GET 'data/tickets.json' | app/features/tickets/tickets.store.ts:415 |
| Local/mock HTTP asset | GET 'data/support-teams.json' | app/features/tickets/tickets.store.ts:433 |
| Local/mock HTTP asset | GET 'data/ticket-categories.json' | app/features/tickets/tickets.store.ts:440 |
| Local/mock HTTP asset | GET 'data/ticket-agents-summary.json' | app/features/tickets/tickets.store.ts:447 |
| Local/mock HTTP asset | GET 'data/tickets.json' | app/features/tickets/tickets.store.ts:457 |
| Local/mock HTTP asset | GET 'data/ticket-detail-extras.json' | app/features/tickets/tickets.store.ts:462 |
| Unresolved or external HTTP target | GET url | app/shared/components/profile/id-card-popover/id-card-popover.ts:128 |

## Interpretation

- `Matched` means the current Angular source has a static call whose normalized route and HTTP method are implemented by an active backend controller action.
- `Method mismatch` means the route exists in the backend but not for the HTTP method Angular sends; this generally causes `405 Method Not Allowed` unless another route layer changes it.
- `Route missing in backend` means no active controller route with the normalized path was found; inspect for a renamed/removed API, a separate backend, or a dynamically configured route.
- `No exact Angular call` does not prove the backend API is unused overall: it may be consumed by another client, triggered dynamically, or intentionally reserved for future use.
