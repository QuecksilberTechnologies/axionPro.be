BEGIN;

ALTER TABLE axionpro."AttendanceDeviceType"
    ADD COLUMN IF NOT EXISTS "DeviceTypeCode" character varying(30);

INSERT INTO axionpro."AttendanceDeviceType"
    ("DeviceTypeCode", "DeviceType", "Remark", "IsActive", "IsDeviceRegister", "AddedById", "AddedDateTime")
SELECT seed."Code", seed."Name", seed."Remark", TRUE, seed."RequiresRegistration", 1, CURRENT_TIMESTAMP
FROM (VALUES
    ('MOBILE', 'Mobile', 'Attendance marked from an authenticated mobile client.', FALSE),
    ('WEB', 'Web', 'Attendance marked from an authenticated web client.', FALSE),
    ('BIOMETRIC', 'Biometric Device', 'Attendance received from a registered physical attendance device.', TRUE),
    ('MANUAL', 'Manual Entry', 'Attendance entered by an authorized administrator.', FALSE)
) AS seed("Code", "Name", "Remark", "RequiresRegistration")
WHERE NOT EXISTS (
    SELECT 1 FROM axionpro."AttendanceDeviceType" existing
    WHERE upper(btrim(existing."DeviceTypeCode")) = seed."Code");

ALTER TABLE axionpro."AttendanceDeviceType"
    ALTER COLUMN "DeviceTypeCode" SET NOT NULL;
CREATE UNIQUE INDEX IF NOT EXISTS "UX_AttendanceDeviceType_DeviceTypeCode"
    ON axionpro."AttendanceDeviceType" (upper(btrim("DeviceTypeCode")));

ALTER TABLE axionpro."EmployeeAttendancePunch"
    ADD COLUMN IF NOT EXISTS "AttendanceDeviceTypeId" integer;

DO $channel_migration$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'axionpro'
          AND table_name = 'EmployeeAttendancePunch'
          AND column_name = 'Channel') THEN
        EXECUTE $sql$
            UPDATE axionpro."EmployeeAttendancePunch" punch
            SET "AttendanceDeviceTypeId" = device_type."Id"
            FROM axionpro."AttendanceDeviceType" device_type
            WHERE punch."AttendanceDeviceTypeId" IS NULL
              AND upper(btrim(device_type."DeviceTypeCode")) = CASE punch."Channel"
                  WHEN 1 THEN 'MOBILE'
                  WHEN 2 THEN 'WEB'
                  WHEN 3 THEN 'BIOMETRIC'
                  WHEN 4 THEN 'MANUAL'
              END
        $sql$;
    END IF;
END $channel_migration$;

DO $validation$
BEGIN
    IF EXISTS (SELECT 1 FROM axionpro."EmployeeAttendancePunch" WHERE "AttendanceDeviceTypeId" IS NULL) THEN
        RAISE EXCEPTION 'EmployeeAttendancePunch contains an unmapped attendance channel.';
    END IF;
END $validation$;

ALTER TABLE axionpro."EmployeeAttendancePunch"
    ALTER COLUMN "AttendanceDeviceTypeId" SET NOT NULL;
ALTER TABLE axionpro."EmployeeAttendancePunch"
    DROP CONSTRAINT IF EXISTS "CK_EmployeeAttendancePunch_Channel";
ALTER TABLE axionpro."EmployeeAttendancePunch"
    DROP COLUMN IF EXISTS "Channel";
ALTER TABLE axionpro."EmployeeAttendancePunch"
    DROP CONSTRAINT IF EXISTS "FK_EmployeeAttendancePunch_AttendanceDeviceType";
ALTER TABLE axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "FK_EmployeeAttendancePunch_AttendanceDeviceType"
    FOREIGN KEY ("AttendanceDeviceTypeId") REFERENCES axionpro."AttendanceDeviceType" ("Id") ON DELETE RESTRICT;

COMMIT;
