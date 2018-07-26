namespace LucenePanGuDemo
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

        /// <summary>类目名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>品牌名
        /// </summary>
        public string BrandName { get; set; }
    }
}
