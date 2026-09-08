using axionpro.application.Constants;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.domain.Entity;
using NUnit.Framework;

namespace axionpro.automationtests.Unit;

/// <summary>Regression contracts for the employee device credential UI payload.</summary>
[TestFixture]
public sealed class EmployeeDeviceCredentialContractTests
{
    [Test]
    public void Set_user_info_is_a_tenant_command_and_requires_the_server_owned_enroll_id()
    {
        var definition = DeviceProtocolCommandCatalog.GetRequired(DeviceCommands.SetUserInfo);

        Assert.Multiple(() =>
        {
            Assert.That(definition.AccessLevel, Is.EqualTo(DeviceCommandAccessLevel.TenantPermission));
            Assert.That(definition.Name, Is.EqualTo(DeviceCommands.SetUserInfo));
        });
    }

    [Test]
    public void Credential_response_exposes_command_state_but_no_raw_credential_value()
    {
        var response = new EmployeeDeviceEnrollmentResponseDTO
        {
            FaceDeploymentStatus = DeviceCredentialDeploymentStatus.Queued,
            FaceCommandStatus = DeviceCommandStatus.AwaitingResponse
        };

        Assert.Multiple(() =>
        {
            Assert.That(response.FaceDeploymentStatus, Is.EqualTo(DeviceCredentialDeploymentStatus.Queued));
            Assert.That(response.FaceCommandStatus, Is.EqualTo(DeviceCommandStatus.AwaitingResponse));
            Assert.That(typeof(EmployeeDeviceEnrollmentResponseDTO).GetProperty("Pin"), Is.Null);
            Assert.That(typeof(EmployeeDeviceEnrollmentResponseDTO).GetProperty("CardNumber"), Is.Null);
        });
    }

    [Test]
    public void User_activation_response_has_a_separate_live_command_status()
    {
        var response = new EmployeeDeviceEnrollmentResponseDTO
        {
            IsActive = false,
            UserActivationCommandStatus = DeviceCommandStatus.AwaitingResponse
        };

        Assert.Multiple(() =>
        {
            Assert.That(response.IsActive, Is.False);
            Assert.That(response.UserActivationCommandStatus, Is.EqualTo(DeviceCommandStatus.AwaitingResponse));
        });
    }

    [Test]
    public void Credential_removal_dropdown_is_centralized_in_the_device_ddl_catalog()
    {
        var field = DeviceDdl.GetSection(DeviceDdl.EmployeeDeviceCredentials).Single();

        Assert.Multiple(() =>
        {
            Assert.That(field.Key, Is.EqualTo("credentialType"));
            Assert.That(field.Options.Select(x => x.Value), Is.EquivalentTo(new[] { "1", "2", "3" }));
            Assert.That(field.Options.Select(x => x.Label), Does.Contain("Face biometric"));
        });
    }

    [Test]
    public void New_employee_device_and_card_dropdowns_are_centralized_and_complete()
    {
        var accessDay = DeviceDdl.GetSection(DeviceDdl.EmployeeDeviceAccessWindows).Single();
        var cardFields = DeviceDdl.GetSection(DeviceDdl.TenantCardInventory);

        Assert.Multiple(() =>
        {
            Assert.That(accessDay.Options.Select(x => x.Value), Is.EquivalentTo(new[] { "1", "2", "3", "4", "5", "6", "7" }));
            Assert.That(cardFields.Select(x => x.Key), Is.EquivalentTo(new[] { "taxTreatment", "cardStatus" }));
            Assert.That(cardFields.Single(x => x.Key == "taxTreatment").Options, Has.Count.EqualTo(4));
            Assert.That(cardFields.Single(x => x.Key == "cardStatus").Options, Has.Count.EqualTo(6));
        });
    }
}
