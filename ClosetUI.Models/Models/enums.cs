namespace ClosetUI.Models.Models;

public enum UserType
{
    HeadOffice = 1,
    HostelMngr = 2,
    HostelTeam = 3
}

public enum Langu
{
    EN = 1,
    HE = 2
}

public enum ObjectType
{
    USR = 1,
    WRKR = 2,
    HOSTL = 3,
    NaN = 4
}

public enum Status
{
    Active = 1,
    Deactivate = 2
}

public enum Colour
{
    Red = 1,
    Blue = 2
}

public enum AtndcType
{
    Type = 1,
    Type2 = 2

}
public enum CommuTypeS
{
    Phone = 1,
    Email = 2,
    Fax = 3,
    WhatsApp = 3
}

public enum HostelManagerTypeEnum
{
    Level01 = 1,
    Level02 = 2,
    Level03 = 3,
    Level04 = 4
}

public enum WrkrTypeEnum
{
    WT1 = 1,
    WT2 = 2,
}

public enum GenderEnum
{
    Male = 1,
    Female = 2,
    Other = 3
}

public enum MrgStatus
{
    Single = 1,
    married = 2
}

public enum SignatureMode
{
    Mngr = 1,
    Wrkr = 2
}


public enum ReportType
{
    NaN = 1,
    PlannedShifts = 2,
    PlannedShiftsFinc = 3,
    ActualShifts = 4,
    ActualShiftsFinc = 5
}
