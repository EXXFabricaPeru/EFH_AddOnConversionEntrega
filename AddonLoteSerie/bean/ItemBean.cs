namespace AddonConEntrega.bean
{
    public class ItemBean
    {
        public string ItemCode { get; set; }
        public string SerieBatch { get; set; }
        public double Quantity { get; set; } = 1;
        public string DueDate { get; set; }
        public string SerieBatchOri { get; set; } = "";
        public bool Selected { get; set; } = false;

    }
}
