using Application.DTOs.Transfer;

namespace Application.DTOs
{
    public class CoverSheetNDeliveryNoteDto
    {
        public List<DeliveryNoteModel> DeliveryNotes { get; set; } = new();
        public List<PickingSlipInfos> DataTransfer { get; set; } = new();

    }
}
