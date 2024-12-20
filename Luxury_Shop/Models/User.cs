﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Luxury_Shop.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.Orders = new HashSet<Order>();
        }
        public int UserID { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập"), StringLength(50)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email"), StringLength(100)]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ tên"), StringLength(100)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại"), StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ"), StringLength(200)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Ngày tạo tài khoản")]
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public Nullable<bool> IsAdmin { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
