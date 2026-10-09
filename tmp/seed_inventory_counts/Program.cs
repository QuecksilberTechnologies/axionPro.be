using System.Text.Json;
using Npgsql;

var tables = new[]{"AttendanceDeviceType","AttendancePolicyVersionConfiguration","BillingTaxRule","ClientType","ComplianceTypeMaster","Country","CountryIdentityRule","DataViewStructure","DefaultEmailConfig","District","EmailTemplate","EmployeeType","EmployeeTypeBasicMenu","Gender","Holiday","HostBillingConfiguration","HostRole","HostRoleModuleAndPermission","HostUser","IdentityCategory","IdentityCategoryDocument","LeaveType","Locality","LocalityType","Module","ModuleOperationMapping","Operation","PageTypeEnum","PaymentGateway","PlanModuleMapping","PolicyCategory","PolicyCategoryRuleType","PolicyDocumentType","PolicyRuleSettingDefinition","PolicyRuleSettingDependency","PolicyRuleSettingOption","PolicyRuleType","PolicyStatus","RoleModuleAndPermission","State","StatutoryType","SubscriptionPlan","TenantEmailTemplate","TenantEnabledModule","TenantEnabledOperation","TenantIndustry","TenantLocation","TenderStatus","Department","Designation","AssetCategory","AssetStatus","AssetType","AssignmentStatus","Category","ComplianceRule","CountryStatutoryRule","ReportingType","RequestType","SalaryComponentMaster","TaxRegimeMaster","TaxRule","TaxSlab","TaxSystemMaster","TicketClassification","TicketType","WorkDocumentType"};
static string ReadConnection(string path){ var text=File.ReadAllText(path); var m=System.Text.RegularExpressions.Regex.Match(text, "(?m)^\\s*\\\"DefaultConnection\\\"\\s*:\\s*\\\"([^\\\"]+)\\\""); if(!m.Success) throw new Exception("Connection not found"); return m.Groups[1].Value; }
var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..","..","..","..",".."));
var targets = new Dictionary<string,string>{{"Local", ReadConnection(Path.Combine(root,"axionpro.api","appsettings.Development.json"))},{"Render",ReadConnection(Path.Combine(root,"axionpro.api","appsettings.Production.json"))}};
var result = new SortedDictionary<string, Dictionary<string,long>>(); foreach(var t in tables) result[t]=new();
foreach(var env in targets){ await using var c=new NpgsqlConnection(env.Value); await c.OpenAsync(); foreach(var t in tables){ await using var cmd=new NpgsqlCommand($"SELECT COUNT(*) FROM axionpro.\"{t}\"",c); result[t][env.Key]=Convert.ToInt64(await cmd.ExecuteScalarAsync()); }}
Console.WriteLine(JsonSerializer.Serialize(result,new JsonSerializerOptions{WriteIndented=true}));


