using HT_Web.Models.Traceability;

namespace HT_Web.Services.Traceability;

public class DummyTraceabilityService
{
    public List<TraceabilityRecord> GetTraceability()
    {
        return new()
        {
            new TraceabilityRecord
            {
                Reference = "35900-63UA0",
                TravelTicketLabel = "TT001",
                Station = "CLIP",
                SecondLabel = "LBL001",
                ThirdTravelTicketLabel = "LBL002",
                FourthTravelTicketLabel = "LBL003",
                FinalLabel = "1111240922530136610-63UA0",
                TestDate = DateTime.Now.AddMinutes(-120),
                Reworked = "No",
                ReworkId = "",
                Hostname = "MOFZETS001"
            },
            new TraceabilityRecord
            {
                Reference = "35900-63UA0",
                TravelTicketLabel = "TT001",
                Station = "ET",
                SecondLabel = "LBL001",
                ThirdTravelTicketLabel = "LBL002",
                FourthTravelTicketLabel = "LBL003",
                FinalLabel = "1111240922530136610-63UA0",
                TestDate = DateTime.Now.AddMinutes(-60),
                Reworked = "No",
                ReworkId = "",
                Hostname = "MOFZETS010"
            }
        };
    }

    public List<SolvedErrorRecord> GetErrors()
    {
        return new()
        {
            new SolvedErrorRecord
            {
                Reference = "35900-63UA0",
                Batch = "B001",
                TravelTicketLabel = "TT001",
                WorkerId = "12345",
                ErrorType = 0,
                Tmp01 = "TMP01",
                Tmp02 = "TMP02",
                Object = "ObjectA",
                Wire1 = "W1",
                Wire2 = "W2",
                Way = "A",
                StartDate = DateTime.Now.AddMinutes(-50),
                EndDate = DateTime.Now.AddMinutes(-45),
                Hostname = "MOFZETS010"
            }
        };
    }
}