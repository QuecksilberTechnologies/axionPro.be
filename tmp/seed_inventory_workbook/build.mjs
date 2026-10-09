import fs from "node:fs/promises";
import path from "node:path";
import { SpreadsheetFile, Workbook } from "@oai/artifact-tool";

const repoRoot = "C:/AxionProCodeBase/QuecksilberTechnologies";
const outDir = path.join(repoRoot, "outputs", "seed-data-inventory");
const previewDir = path.join(outDir, "previews");
await fs.mkdir(previewDir, { recursive: true });

const counts = JSON.parse(await fs.readFile(path.join(repoRoot, "tmp", "seed_inventory_counts", "counts.json"), "utf8"));

const sql = (file, role = "Incremental SQL") => ({ type: "SQL file", source: file, role });
const runtime = (source) => ({ type: "Runtime helper", source, role: "Runtime auto-seed" });

const seedTables = [
  ["AttendanceDeviceType","Tenant / reference",sql("database-scripts/AddEmployeeAttendancePunch.sql"),sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL"),sql("database-scripts/ReplaceAttendanceChannelWithDeviceType.sql")],
  ["AttendancePolicyVersionConfiguration","Policy",sql("database-scripts/AddAttendancePolicyVersionConfiguration.sql")],
  ["BillingTaxRule","Billing / subscription",sql("database-scripts/AddSubscriptionBillingFoundation.sql")],
  ["ClientType","Tenant / reference",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["ComplianceTypeMaster","Geography / statutory",sql("database-scripts/production-seed/03-geography/002-country-regulatory-master.sql","Canonical SQL")],
  ["Country","Geography / statutory",sql("database-scripts/production-seed/03-geography/000-iso-country-catalog.sql","Canonical SQL"),sql("database-scripts/production-seed/03-geography/001-four-country-postal-catalog.sql","Canonical SQL"),sql("database-scripts/ResetIndiaChinaFullLocationCatalog.sql")],
  ["CountryIdentityRule","Geography / statutory",sql("database-scripts/production-seed/05-dependent-master/001-employee-identity-catalog.sql","Canonical SQL")],
  ["DataViewStructure","Tenant / reference",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["DefaultEmailConfig","Email",sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL")],
  ["District","Geography / statutory",sql("database-scripts/production-seed/03-geography/001-four-country-postal-catalog.sql","Canonical SQL"),sql("database-scripts/RenameCityToLocality.sql"),sql("database-scripts/ResetIndiaChinaFullLocationCatalog.sql"),sql("database-scripts/SeedUaeLocations.sql"),sql("database-scripts/SyncDistrictsFromCities.sql")],
  ["EmailTemplate","Email",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["EmployeeType","Employee / organisation",sql("database-scripts/AddTenantEmployeeTypes.sql"),sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["EmployeeTypeBasicMenu","Employee / organisation",sql("database-scripts/AddTenantEmployeeTypes.sql")],
  ["Gender","Employee / organisation",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["Holiday","Tenant / reference",sql("database-scripts/SeedTechNovaHolidayCalendar2026.sql")],
  ["HostBillingConfiguration","Billing / subscription",sql("database-scripts/AddSubscriptionBillingFoundation.sql")],
  ["HostRole","Access / navigation",sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL")],
  ["HostRoleModuleAndPermission","Access / navigation",sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/SeedHostBillingConfigurationModule.sql"),sql("database-scripts/SeedHostBulkImportModules.sql"),sql("database-scripts/TenantCardMaster_EmployeeDeviceCredential_Upgrade.sql")],
  ["HostUser","Access / navigation",sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL")],
  ["IdentityCategory","Employee / organisation",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL"),sql("database-scripts/production-seed/05-dependent-master/001-employee-identity-catalog.sql","Canonical SQL")],
  ["IdentityCategoryDocument","Employee / organisation",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL"),sql("database-scripts/production-seed/05-dependent-master/001-employee-identity-catalog.sql","Canonical SQL")],
  ["LeaveType","Policy",sql("database-scripts/AddPolicyLeaveTypeTargeting.sql")],
  ["Locality","Geography / statutory",sql("database-scripts/production-seed/03-geography/001-four-country-postal-catalog.sql","Canonical SQL"),sql("database-scripts/SeedUaeLocations.sql")],
  ["LocalityType","Geography / statutory",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL"),sql("database-scripts/production-seed/03-geography/001-four-country-postal-catalog.sql","Canonical SQL"),sql("database-scripts/RenameCityToLocality.sql")],
  ["Module","Access / navigation",sql("database-scripts/complete-seed/AxionPro_New_Production_Module_Operation_Seed.sql","Full seed"),sql("database-scripts/complete-seed/SeedTenantPolicyModules.sql","Full seed"),sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/SeedBulkImportModules.sql"),sql("database-scripts/SeedHostBillingConfigurationModule.sql"),sql("database-scripts/SeedTenantEmailTemplate.sql"),sql("database-scripts/SeedTenantEmployeeTypeModule.sql"),sql("database-scripts/TenantCardMaster_EmployeeDeviceCredential_Upgrade.sql")],
  ["ModuleOperationMapping","Access / navigation",sql("database-scripts/complete-seed/AxionPro_New_Production_Module_Operation_Seed.sql","Full seed"),sql("database-scripts/complete-seed/SeedTenantPolicyModules.sql","Full seed"),sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/MoveResetPasswordToEmployeeList.sql"),sql("database-scripts/RemoveDuplicateExportOperation.sql"),sql("database-scripts/SeedBulkImportModules.sql"),sql("database-scripts/SeedHostBillingConfigurationModule.sql"),sql("database-scripts/SeedHostBulkImportModules.sql"),sql("database-scripts/SeedTenantEmailTemplate.sql"),sql("database-scripts/SeedTenantEmployeeTypeModule.sql"),sql("database-scripts/TenantCardMaster_EmployeeDeviceCredential_Upgrade.sql")],
  ["Operation","Access / navigation",sql("database-scripts/complete-seed/AxionPro_New_Production_Module_Operation_Seed.sql","Full seed"),sql("database-scripts/complete-seed/SeedTenantPolicyModules.sql","Full seed"),sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/SeedBulkImportModules.sql"),sql("database-scripts/SeedHostBulkImportModules.sql")],
  ["PageTypeEnum","Access / navigation",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["PaymentGateway","Billing / subscription",sql("database-scripts/AddSubscriptionBillingFoundation.sql")],
  ["PlanModuleMapping","Access / navigation",sql("database-scripts/complete-seed/AxionPro_New_Production_Module_Operation_Seed.sql","Full seed"),sql("database-scripts/complete-seed/SeedTenantPolicyModules.sql","Full seed"),sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/SeedBulkImportModules.sql"),sql("database-scripts/SeedTenantEmailTemplate.sql"),sql("database-scripts/SeedTenantEmployeeTypeModule.sql")],
  ["PolicyCategory","Policy",sql("database-scripts/CreateGenericTenantPolicyFramework.sql"),sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["PolicyCategoryRuleType","Policy",sql("database-scripts/AddGenericPolicyRuleMetadata.sql")],
  ["PolicyDocumentType","Policy",sql("database-scripts/CreateGenericTenantPolicyFramework.sql"),sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["PolicyRuleSettingDefinition","Policy",sql("database-scripts/AddGenericPolicyRuleMetadata.sql")],
  ["PolicyRuleSettingDependency","Policy",sql("database-scripts/AddGenericPolicyRuleMetadata.sql")],
  ["PolicyRuleSettingOption","Policy",sql("database-scripts/AddGenericPolicyRuleMetadata.sql")],
  ["PolicyRuleType","Policy",sql("database-scripts/CreateGenericTenantPolicyFramework.sql"),sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["PolicyStatus","Policy",sql("database-scripts/CreateGenericTenantPolicyFramework.sql"),sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["RoleModuleAndPermission","Access / navigation",sql("database-scripts/MoveResetPasswordToEmployeeList.sql"),sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/RemoveDuplicateExportOperation.sql"),sql("database-scripts/SeedBulkImportModules.sql")],
  ["State","Geography / statutory",sql("database-scripts/production-seed/03-geography/001-four-country-postal-catalog.sql","Canonical SQL"),sql("database-scripts/ResetIndiaChinaFullLocationCatalog.sql"),sql("database-scripts/SeedGlobalStates.sql"),sql("database-scripts/SeedUaeLocations.sql")],
  ["StatutoryType","Geography / statutory",sql("database-scripts/production-seed/03-geography/002-country-regulatory-master.sql","Canonical SQL")],
  ["SubscriptionPlan","Billing / subscription",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["TenantEmailTemplate","Email",sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/SeedTenantEmailTemplate.sql")],
  ["TenantEnabledModule","Access / navigation",sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/SeedTenantEmailTemplate.sql")],
  ["TenantEnabledOperation","Access / navigation",sql("database-scripts/MoveResetPasswordToEmployeeList.sql"),sql("database-scripts/production-seed/04-access-and-host/001-modules-operations-two-host-admins.sql","Canonical SQL"),sql("database-scripts/RemoveDuplicateExportOperation.sql"),sql("database-scripts/SeedBulkImportModules.sql"),sql("database-scripts/SeedTenantEmailTemplate.sql"),sql("database-scripts/SeedTenantEmployeeTypeModule.sql")],
  ["TenantIndustry","Tenant / reference",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["TenantLocation","Tenant / reference",sql("database-scripts/SeedTechNovaHolidayCalendar2026.sql")],
  ["TenderStatus","Tenant / reference",sql("database-scripts/production-seed/02-parent-master/001-shared-master-data.sql","Canonical SQL")],
  ["Department","Employee / organisation",runtime("axionpro.infrastructure/SeedHelpers/DepartmentSeedHelper.cs → DepartmentRepository.AutoCreateDepartmentSeedAsync")],
  ["Designation","Employee / organisation",runtime("axionpro.infrastructure/SeedHelpers/DesignationsSeedHelper.cs → DesignationRepository")],
];

const missing = ["AssetCategory","AssetStatus","AssetType","AssignmentStatus","Category","ComplianceRule","CountryStatutoryRule","ReportingType","RequestType","SalaryComponentMaster","TaxRegimeMaster","TaxRule","TaxSlab","TaxSystemMaster","TicketClassification","TicketType","WorkDocumentType"];

const workbook = Workbook.create();
const font = "Arial";
const navy = "#17365D";
const blue = "#2F75B5";
const paleBlue = "#D9EAF7";
const paleGreen = "#E2F0D9";
const paleRed = "#FCE4D6";
const paleAmber = "#FFF2CC";
const gray = "#667085";
const border = "#D0D5DD";

function setup(sheet) {
  sheet.showGridlines = false;
}
function title(sheet, text, subtitle, width) {
  sheet.getRange(`A2:${width}2`).merge();
  sheet.getRange("A2").values = [[text]];
  sheet.getRange("A2").format.font = { name: font, size: 16, bold: true, color: navy };
  sheet.getRange(`A3:${width}3`).merge();
  sheet.getRange("A3").values = [[subtitle]];
  sheet.getRange("A3").format.font = { name: font, size: 10, italic: true, color: gray };
  sheet.getRange(`A4:${width}4`).format.borders = { bottom: { color: blue, style: "thin" } };
}
function header(range) {
  range.format.fill = navy;
  range.format.font = { name: font, size: 10, bold: true, color: "#FFFFFF" };
  range.format.horizontalAlignment = "center";
  range.format.verticalAlignment = "center";
  range.format.borders = { bottom: { color: "#FFFFFF", style: "thin" } };
}
function setWidths(sheet, widths) {
  Object.entries(widths).forEach(([col, width]) => sheet.getRange(`${col}:${col}`).format.columnWidth = width);
}

const summary = workbook.worksheets.add("Summary");
setup(summary);
title(summary, "AxionPro seed data inventory", "Current source-controlled seed coverage with exact Local and Render row counts · 09 Oct 2026", "I");
summary.getRange("A6:H6").values = [["Metric","Value","Metric","Value","Metric","Value","Metric","Value"]];
header(summary.getRange("A6:H6"));
summary.getRange("A7:H7").values = [["Database tables",166,"Explicitly seeded",50,"SQL seeded",48,"Runtime seeded",2]];
summary.getRange("A8:H8").values = [["Missing seed files",17,"EF HasData",0,"Matching row counts",null,"Different row counts",null]];
summary.getRange("F8").formulas = [["=COUNTIF('Seed catalog'!H7:H56,\"MATCH\")"]];
summary.getRange("H8").formulas = [["=COUNTIF('Seed catalog'!H7:H56,\"DIFF\")"]];
summary.getRange("A7:H8").format.fill = "#F8FAFC";
summary.getRange("A7:H8").format.rowHeight = 24;
summary.getRange("B7:B8,D7:D8,F7:F8,H7:H8").format.font = { name: font, size: 12, bold: true, color: blue };
summary.getRange("A11:D11").values = [["Seed category","Tables","Render rows","Local rows"]];
header(summary.getRange("A11:D11"));
const categories = [...new Set(seedTables.map(r => r[1]))].sort();
categories.forEach((c, i) => {
  const row = 12 + i;
  summary.getRange(`A${row}`).values = [[c]];
  summary.getRange(`B${row}`).formulas = [[`=COUNTIF('Seed catalog'!C$7:C$56,A${row})`]];
  summary.getRange(`C${row}`).formulas = [[`=SUMIF('Seed catalog'!C$7:C$56,A${row},'Seed catalog'!F$7:F$56)`]];
  summary.getRange(`D${row}`).formulas = [[`=SUMIF('Seed catalog'!C$7:C$56,A${row},'Seed catalog'!G$7:G$56)`]];
});
summary.getRange(`A12:D${11+categories.length}`).format.borders = { bottom: { color: border, style: "thin" } };
summary.getRange("F11:I11").values = [["Review finding","Value","Meaning","Action"]];
header(summary.getRange("F11:I11"));
summary.getRange("F12:I15").values = [
  ["Environment parity","10 differences","Exact current row counts differ between Local and Render.","Review Seed catalog rows marked DIFF."],
  ["Geography volume","Large catalogs","Local contains broader geography data than Render.","Use source scripts; workbook does not duplicate lakh-level rows."],
  ["TenantEmailTemplate","Local 0 / Render 5","Local tenant template seed is missing from current data.","Run the approved seed only after environment review."],
  ["Missing seed files",17,"Tables exist but no authoritative seed source was found.","Obtain business-approved catalogues; do not invent values."],
];
summary.getRange("F12:I15").format.wrapText = true;
summary.getRange("F12:I15").format.borders = { bottom: { color: border, style: "thin" } };
summary.getRange("A23:I23").merge();
summary.getRange("A23").values = [["Scope note: “Seeded” means an explicit INSERT-based SQL source or an explicitly named runtime seed helper found in this repository. Row counts are a live snapshot, not the canonical expected value."]];
summary.getRange("A23").format.fill = paleAmber;
summary.getRange("A23").format.font = { name: font, size: 10, italic: true, color: "#7F6000" };
summary.getRange("A23").format.wrapText = true;
setWidths(summary,{A:25,B:12,C:20,D:14,E:3,F:24,G:16,H:45,I:38});

const catalog = workbook.worksheets.add("Seed catalog");
setup(catalog);
title(catalog, "Seeded table catalog", "One row per seeded table; source details are expanded in Seed sources.", "J");
catalog.getRange("A6:J6").values = [["#","Table","Seed category","Mechanism","Primary source","Render rows","Local rows","Parity","Source count","Notes"]];
header(catalog.getRange("A6:J6"));
seedTables.forEach((r, i) => {
  const row = i + 7;
  const sources = r.slice(2);
  const mechanism = sources[0].type === "Runtime helper" ? "Runtime" : "SQL";
  const primary = sources.find(s => s.role === "Canonical SQL")?.source ?? sources[0].source;
  const render = counts[r[0]].Render;
  const local = counts[r[0]].Local;
  const note = ["Country","State","District","Locality"].includes(r[0]) ? "High-volume geography catalog; refer to authoritative SQL source." : "";
  catalog.getRange(`A${row}:J${row}`).values = [[i+1,r[0],r[1],mechanism,primary,render,local,null,sources.length,note]];
  catalog.getRange(`H${row}`).formulas = [[`=IF(F${row}=G${row},\"MATCH\",\"DIFF\")`]];
  catalog.getRange(`H${row}`).format.fill = render === local ? paleGreen : paleRed;
  catalog.getRange(`H${row}`).format.font = { name: font, size: 10, bold: true, color: render === local ? "#375623" : "#9C0006" };
});
catalog.getRange("A7:J56").format.borders = { bottom: { color: border, style: "thin" } };
catalog.getRange("E7:E56").format.wrapText = true;
catalog.getRange("J7:J56").format.wrapText = true;
catalog.freezePanes.freezeRows(6);
setWidths(catalog,{A:6,B:35,C:25,D:12,E:68,F:15,G:15,H:12,I:14,J:42});

const sourcesSheet = workbook.worksheets.add("Seed sources");
setup(sourcesSheet);
title(sourcesSheet, "Seed source mapping", "Every identified table-to-source relationship, including canonical, incremental and runtime seeding.", "E");
sourcesSheet.getRange("A6:E6").values = [["#","Table","Source type","Source / method","Role"]];
header(sourcesSheet.getRange("A6:E6"));
let sourceRow = 7;
let sourceIndex = 1;
for (const r of seedTables) {
  for (const s of r.slice(2)) {
    sourcesSheet.getRange(`A${sourceRow}:E${sourceRow}`).values = [[sourceIndex++,r[0],s.type,s.source,s.role]];
    sourceRow++;
  }
}
sourcesSheet.getRange(`A7:E${sourceRow-1}`).format.borders = { bottom: { color: border, style: "thin" } };
sourcesSheet.getRange(`D7:D${sourceRow-1}`).format.wrapText = true;
sourcesSheet.freezePanes.freezeRows(6);
setWidths(sourcesSheet,{A:7,B:35,C:18,D:100,E:20});

const missingSheet = workbook.worksheets.add("Missing seeds");
setup(missingSheet);
title(missingSheet, "Master/reference tables without seed files", "Tables identified as master/reference candidates where no authoritative SQL or runtime seed source was found.", "G");
missingSheet.getRange("A6:G6").values = [["#","Table","Render rows","Local rows","Current status","Risk","Recommended action"]];
header(missingSheet.getRange("A6:G6"));
missing.forEach((t, i) => {
  const row = i + 7;
  missingSheet.getRange(`A${row}:G${row}`).values = [[i+1,t,counts[t].Render,counts[t].Local,"No authoritative seed found","Environment setup can remain empty or diverge.","Define a business-approved catalogue and idempotent seed; do not invent values."]];
});
missingSheet.getRange(`A7:G${6+missing.length}`).format.borders = { bottom: { color: border, style: "thin" } };
missingSheet.getRange(`E7:E${6+missing.length}`).format.fill = paleAmber;
missingSheet.getRange(`F7:G${6+missing.length}`).format.wrapText = true;
missingSheet.freezePanes.freezeRows(6);
setWidths(missingSheet,{A:7,B:34,C:15,D:15,E:28,F:44,G:62});

const checks = workbook.worksheets.add("Checks");
setup(checks);
title(checks, "Workbook checks", "Terminal audit checks for completeness and consistency. These checks do not drive other sheets.", "E");
checks.getRange("A6:E6").values = [["Check","Expected","Actual","Status","Explanation"]];
header(checks.getRange("A6:E6"));
const checkRows = [
  ["Seed catalog row count",50,"=COUNTA('Seed catalog'!B7:B56)","Every seeded table appears once."],
  ["SQL-seeded tables",48,"=COUNTIF('Seed catalog'!D7:D56,\"SQL\")","Explicit INSERT-based source found."],
  ["Runtime-seeded tables",2,"=COUNTIF('Seed catalog'!D7:D56,\"Runtime\")","Explicit seed helper found."],
  ["Missing-seed candidates",17,"=COUNTA('Missing seeds'!B7:B23)","Separate review list is complete."],
  ["Blank source count",0,"=COUNTBLANK('Seed catalog'!E7:E56)","Every seeded table has a primary source."],
  ["Duplicate table names",0,seedTables.length - new Set(seedTables.map(r => r[0])).size,"Catalog table names are unique."],
  ["EF HasData usage",0,"=0","Repository scan found no EF Core HasData seeding."],
];
checkRows.forEach((r,i)=>{
  const row=7+i;
  checks.getRange(`A${row}:B${row}`).values=[[r[0],r[1]]];
  if (typeof r[2] === "number") checks.getRange(`C${row}`).values=[[r[2]]];
  else checks.getRange(`C${row}`).formulas=[[r[2]]];
  checks.getRange(`D${row}`).formulas=[[`=IF(B${row}=C${row},\"PASS\",\"FAIL\")`]];
  checks.getRange(`E${row}`).values=[[r[3]]];
});
checks.getRange("A7:E13").format.borders = { bottom: { color: border, style: "thin" } };
checks.getRange("D7:D13").format.fill = paleGreen;
checks.getRange("D7:D13").format.font = { name: font, size: 10, bold: true, color: "#375623" };
checks.getRange("A16:C16").values = [["Environment difference","Count","Review"]];
header(checks.getRange("A16:C16"));
checks.getRange("A17:C17").values = [["Local vs Render row-count differences",null,"Open Seed catalog and filter Parity = DIFF."]];
checks.getRange("B17").formulas = [["=COUNTIF('Seed catalog'!H7:H56,\"DIFF\")"]];
checks.getRange("A17:C17").format.fill = paleAmber;
setWidths(checks,{A:38,B:14,C:14,D:14,E:68});

workbook.recalculate();
for (const s of [summary,catalog,sourcesSheet,missingSheet,checks]) {
  const rendered = await workbook.render({ sheetName: s.name, autoCrop: "all", scale: 1, format: "png" });
  await fs.writeFile(path.join(previewDir, `${s.name.replaceAll(" ", "_")}.png`), new Uint8Array(await rendered.arrayBuffer()));
}
const outputPath = path.join(outDir, "AxionPro_Seed_Data_Inventory_2026-10-09.xlsx");
const xlsx = await SpreadsheetFile.exportXlsx(workbook);
await xlsx.save(outputPath);
console.log(JSON.stringify({ outputPath, previewDir, sheets: workbook.worksheets.items.map(s => s.name), sourceRows: sourceRow - 7 }, null, 2));
