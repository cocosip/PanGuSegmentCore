namespace PanGuLuceneDemo
{
    public class Product
    {
        public int Id { get; set; }

        /// <summary>商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>商品前缀名称
        /// </summary>
        public string PrefixName { get; set; }

        /// <summary>商品后缀名称
        /// </summary>
        public string SuffixName { get; set; }

        /// <summary>商品编号
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>所属类目
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>类目名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>所属品牌
        /// </summary>
        public int BrandId { get; set; }

        /// <summary>品牌名称
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>商品详情,预留
        /// </summary>
        public string Detail { get; set; }
    }
}
