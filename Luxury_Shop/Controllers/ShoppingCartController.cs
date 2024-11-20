using Luxury_Shop.Models;
using System.Linq;
using System.Web.Mvc;

public class ShoppingCartController : Controller
{
    private LuxuryEntities1 database = new LuxuryEntities1();
    
    // GET: ShoppingCart/ShowCart
    public ActionResult ShowCart()
    {
        // Lấy giỏ hàng từ session
        var cart = Session["Cart"] as Cart ?? new Cart();

        var orderViewModel = new OrderViewModel
        {
            Cart = cart // Gán giỏ hàng cho OrderViewModel
        };

        return View(orderViewModel); // Truyền OrderViewModel vào view
    }


    // POST: ShoppingCart/AddToCart
    [HttpPost]
    
    public ActionResult AddToCart(int productId, int quantity = 1,string Size="")
    {
        // Kiểm tra sản phẩm có tồn tại trong cơ sở dữ liệu không
        var product = database.Products.FirstOrDefault(p => p.ProductID == productId);
        if (product == null)
        {
            return HttpNotFound("Product not found");
        }

        // Lấy giỏ hàng từ session hoặc tạo mới nếu không có
        var cart = Session["Cart"] as Cart ?? new Cart();

        // Thêm sản phẩm vào giỏ hàng
        cart.AddToCart(product, quantity,Size);

        // Lưu giỏ hàng vào session
        Session["Cart"] = cart;

        return RedirectToAction("ShowCart");
    }
    
    // GET: ShoppingCart/Checkout
    public ActionResult Checkout()
    {
        var cart = Session["Cart"] as Cart;

        // Kiểm tra nếu giỏ hàng rỗng, quay lại trang giỏ hàng
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("ShowCart");
        }

        // Hiển thị giỏ hàng để người dùng kiểm tra
        return View(cart);
    }

    // POST: ShoppingCart/ProcessCheckout
    [HttpPost]
    public ActionResult ProcessCheckout()
    {
        var cart = Session["Cart"] as Cart;

        // Kiểm tra nếu giỏ hàng trống, quay lại trang giỏ hàng
        if (cart == null || !cart.Items.Any())
        {
            return RedirectToAction("ShowCart");
        }

        // Xử lý thanh toán (giả sử bạn có phương thức thanh toán tại đây)
        // ...

        // Sau khi thanh toán thành công, xóa giỏ hàng trong session
        Session["Cart"] = null;

        // Chuyển tới trang xác nhận đơn hàng
        return RedirectToAction("OrderConfirmation");
    }
    public ActionResult Update_Cart_Quantity(FormCollection form)
    {
        Cart cart = Session["Cart"] as Cart;
        int id_pro = int.Parse(Request.Form["idPro"]);
        int _quantity = int.Parse(Request.Form["carQuantity"]);
        cart.Update_quantity(id_pro, _quantity);

        return RedirectToAction("ShowCart", "ShoppingCart");
    }
    // GET: ShoppingCart/OrderConfirmation
    public ActionResult OrderConfirmation()
    {
        return View();
    }

    // Phương thức tính tổng số tiền trong giỏ hàng
    public decimal GetTotalAmount()
    {
        var cart = Session["Cart"] as Cart;

        // Nếu không có giỏ hàng hoặc giỏ hàng trống, trả về 0
        if (cart == null || !cart.Items.Any())
        {
            return 0;
        }

        // Tính tổng số tiền của các sản phẩm trong giỏ hàng
        return cart.Items.Sum(item => item.Product.OriginalPrice * item.Quantity);
    }
    // POST: ShoppingCart/RemoveFromCart
    [HttpPost]
    public ActionResult RemoveFromCart(int productId)
    {
        // Lấy giỏ hàng từ session
        var cart = Session["Cart"] as Cart ?? new Cart();

        // Xóa sản phẩm khỏi giỏ hàng
        cart.RemoveFromCart(productId);

        // Lưu lại giỏ hàng vào session
        Session["Cart"] = cart;

        // Chuyển hướng về trang hiển thị giỏ hàng
        return RedirectToAction("ShowCart");
    }

}
