using System.Collections.Generic;
using GR.UI.Menu.Abstractions.Helpers;

namespace GR.ECommerce.Razor.Helpers
{
    internal class CommerceMenuInitializer : BaseMenuInitializer
    {
        public override MenuInitBuilder Builder => new MenuInitBuilder
        {
            MenuGroup = MenuResources.AppMenuId,
            Configs = new List<MenuItemConfig>
            {
                new MenuItemConfig(CommerceRazorResources.Menu.CommerceMenuItem)
                {
                    ParentMenuItemId = MenuResources.MenuItems.AdministrationItem,
                    Name = "Commerce",
                    Translate = "system_commerce",
                    Href = MenuResources.MenuItems.None
                },

                new MenuItemConfig(CommerceRazorResources.Menu.CatalogMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CommerceMenuItem,
                    Name = "Catalog",
                    Translate = "system_commerce_catalog",
                    Href = MenuResources.MenuItems.None
                },
                new MenuItemConfig(CommerceRazorResources.Menu.ProductsMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CatalogMenuItem,
                    Name = "Products",
                    Translate = "system_commerce_products",
                    Href = "/products"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.BrandsMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CatalogMenuItem,
                    Name = "Brands",
                    Translate = "system_commerce_brands",
                    Href = "/brand"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.ProductCategoriesMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CatalogMenuItem,
                    Name = "Product categories",
                    Translate = "system_commerce_category",
                    Href = "/productcategory"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.ProductOptionsMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CatalogMenuItem,
                    Name = "Product options",
                    Translate = "none",
                    Href = "/ProductOption"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.ProductTypesMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CatalogMenuItem,
                    Name = "Product types",
                    Translate = "system_commerce_product_types",
                    Href = "/producttype"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.AttributeGroupsMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CatalogMenuItem,
                    Name = "Attribute Groups",
                    Translate = "system_commerce_attribute_group",
                    Href = "/AttributeGroup"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.ProductAttributesMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CatalogMenuItem,
                    Name = "Attributes",
                    Translate = "system_commerce_attributes",
                    Href = "/ProductAttribute"
                },


                new MenuItemConfig(CommerceRazorResources.Menu.SalesMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CommerceMenuItem,
                    Name = "Sales",
                    Translate = "system_commerce_sales",
                    Href = MenuResources.MenuItems.None
                },
                new MenuItemConfig(CommerceRazorResources.Menu.OrdersMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.SalesMenuItem,
                    Name = "Orders",
                    Translate = "system_commerce_orders",
                    Href = "/Orders/AllOrders"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.DashboardMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.SalesMenuItem,
                    Name = "Commerce Dashboard",
                    Translate = "_dashboard",
                    Href = "/products/dashboard"
                },

                new MenuItemConfig(CommerceRazorResources.Menu.StoreMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CommerceMenuItem,
                    Name = "Store",
                    Href = MenuResources.MenuItems.None
                },

                new MenuItemConfig(CommerceRazorResources.Menu.PaymentMethodsMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CommerceMenuItem,
                    Name = "Payment methods",
                    Translate = "system_payment_methods",
                    Href = "/PaymentMethods"
                },
                new MenuItemConfig(CommerceRazorResources.Menu.CommerceSettingsMenuItem)
                {
                    ParentMenuItemId = CommerceRazorResources.Menu.CommerceMenuItem,
                    Name = "Commerce Settings",
                    Translate = "none",
                    Href = "/CommerceSettings"
                }
            }
        };
    }
}