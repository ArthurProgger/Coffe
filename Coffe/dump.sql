USE [master]
GO
/****** Object:  Database [Coffe]    Script Date: 02.04.2023 19:22:37 ******/
CREATE DATABASE [Coffe]
GO
USE [Coffe]
GO
/****** Object:  DatabaseRole [client]    Script Date: 02.04.2023 19:22:38 ******/
CREATE ROLE [client]
GO
/****** Object:  Table [dbo].[crafts]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[crafts](
	[prod_id] [int] NOT NULL,
	[ingr_id] [int] NOT NULL,
	[ingr_count] [float] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ingredients]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ingredients](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ingr_name] [varchar](100) NOT NULL,
	[price] [money] NOT NULL,
	[count_of_price] [float] NOT NULL,
	[unit] [varchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[ingr_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ingredients_coming]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ingredients_coming](
	[coming_datetime] [datetime] NOT NULL,
	[ingr_id] [int] NOT NULL,
	[ingr_count] [float] NOT NULL,
UNIQUE NONCLUSTERED 
(
	[coming_datetime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ingredients_using]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ingredients_using](
	[using_datetime] [datetime] NOT NULL,
	[ingr_id] [int] NOT NULL,
	[ingr_count] [float] NOT NULL,
UNIQUE NONCLUSTERED 
(
	[using_datetime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[orders]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_login] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[orders_products]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orders_products](
	[order_id] [int] NOT NULL,
	[product_id] [int] NOT NULL,
	[is_cooked] [bit] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[products]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[prod_name] [varchar](100) NOT NULL,
	[prod_type] [varchar](60) NOT NULL,
	[price] [money] NOT NULL,
	[img_path] [varchar](256) NULL,
	[recipe] [varchar](max) NULL,
	[cooking_time] [time](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[img_path] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[prod_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[products_types]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[products_types](
	[prod_type_name] [varchar](60) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[prod_type_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[units]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[units](
	[short_name] [varchar](10) NOT NULL,
	[full_name] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[short_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[user_login] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[user_login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[crafts]  WITH CHECK ADD FOREIGN KEY([prod_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[crafts]  WITH CHECK ADD FOREIGN KEY([ingr_id])
REFERENCES [dbo].[ingredients] ([id])
GO
ALTER TABLE [dbo].[ingredients]  WITH CHECK ADD FOREIGN KEY([unit])
REFERENCES [dbo].[units] ([short_name])
GO
ALTER TABLE [dbo].[ingredients_coming]  WITH CHECK ADD FOREIGN KEY([ingr_id])
REFERENCES [dbo].[ingredients] ([id])
GO
ALTER TABLE [dbo].[ingredients_using]  WITH CHECK ADD FOREIGN KEY([ingr_id])
REFERENCES [dbo].[ingredients] ([id])
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD FOREIGN KEY([user_login])
REFERENCES [dbo].[users] ([user_login])
GO
ALTER TABLE [dbo].[orders_products]  WITH CHECK ADD FOREIGN KEY([order_id])
REFERENCES [dbo].[orders] ([id])
GO
ALTER TABLE [dbo].[orders_products]  WITH CHECK ADD FOREIGN KEY([product_id])
REFERENCES [dbo].[products] ([id])
GO
ALTER TABLE [dbo].[products]  WITH CHECK ADD FOREIGN KEY([prod_type])
REFERENCES [dbo].[products_types] ([prod_type_name])
GO
/****** Object:  StoredProcedure [dbo].[get_craft]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[get_craft] @prod_id int as

	select
		crafts.ingr_id,
		crafts.ingr_count
	from	
		crafts
	where
		crafts.prod_id = @prod_id
GO
/****** Object:  StoredProcedure [dbo].[get_ingredients]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[get_ingredients] as

	select
		ingredients.id,
		ingredients.ingr_name as [Наименование ингредиентов],
		ingredients.price as Цена,
		'' as [В наличии],
		ingredients.unit,
		ingredients.count_of_price,
		(select
			count(*)
		from
			ingredients),
		(select
			sum(ingredients.price)
		from
			ingredients)
	from
		ingredients
GO
/****** Object:  StoredProcedure [dbo].[get_ingredients_count]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[get_ingredients_count] @ingr_id int as

	if ((select
			sum(ingr_count)
		from
			ingredients_coming
		where
			ingredients_coming.ingr_id = @ingr_id)  is null)
		select 0
	else
	begin
		if ((select
				sum(ingr_count)
			from
				ingredients_using
			where
				ingredients_using.ingr_id = @ingr_id) is null)
			select
				sum(ingr_count)
			from
				ingredients_coming
			where
				ingredients_coming.ingr_id = @ingr_id
		else
			select
				(select
					sum(ingr_count)
				from
					ingredients_coming
				where
					ingredients_coming.ingr_id = @ingr_id)
				-
				(select
					sum(ingr_count)
				from
					ingredients_using
				where
					ingredients_using.ingr_id = @ingr_id)
	end
GO
/****** Object:  StoredProcedure [dbo].[get_last_order]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[get_last_order] @login nvarchar(100) as
	
	select top(1)
		orders.id
	from
		orders
	where
		orders.user_login = @login
	order by
		orders.id desc
GO
/****** Object:  StoredProcedure [dbo].[get_orders]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[get_orders] as

	select
		orders.id,
		orders.user_login
	from
		orders
	inner join orders_products on orders_products.order_id = orders.id
	where
		orders_products.is_cooked = 0
GO
/****** Object:  StoredProcedure [dbo].[get_product_recipe]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[get_product_recipe] @prod_id int as

	select
		products.recipe
	from
		products
	where
		products.id = @prod_id
GO
/****** Object:  StoredProcedure [dbo].[get_products]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[get_products] @type varchar(60) = '', @name varchar(100) = '', @only_have bit = 0, @order_id int = -1 as

	if (@order_id = -1)
	begin
		if (@only_have = 0)
		begin
			if (@type = '')
				select
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.cooking_time
				from
					products
				where
					products.prod_name like @name + '%'
			else
				select
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.cooking_time
				from
					products
				where
					products.prod_name like @name + '%' and products.prod_type = @type
		end
		else
		begin
			if (@type = '')
				select distinct
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.prod_type,
					products.cooking_time
				from
					products
				left join crafts on crafts.prod_id = products.id
				where
					products.prod_name like @name + '%'
					and
					(select
						sum(ingredients_coming.ingr_count)
					from
						ingredients_coming
					where
						ingredients_coming.ingr_id = crafts.ingr_id)
					-
					(select
						sum(ingredients_using.ingr_count)
					from
						ingredients_using
					where
						ingredients_using.ingr_id = crafts.ingr_id)
					- crafts.ingr_count > 0

			else
				select distinct
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.cooking_time
				from
					products
				left join crafts on crafts.prod_id = products.id
				where
					products.prod_name like @name + '%' and products.prod_type = @type
					and
									(select
						sum(ingredients_coming.ingr_count)
					from
						ingredients_coming
					where
						ingredients_coming.ingr_id = crafts.ingr_id)
					-
					(select
						sum(ingredients_using.ingr_count)
					from
						ingredients_using
					where
						ingredients_using.ingr_id = crafts.ingr_id)
					- crafts.ingr_count > 0
		end
	end
	else
		begin
		if (@only_have = 0)
		begin
			if (@type = '')
				select
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.cooking_time
				from
					products
				where
					products.prod_name like @name + '%'
					and
					exists
					(select
						orders_products.is_cooked
					from
						orders_products
					where
						orders_products.order_id = @order_id
						and
						orders_products.product_id = products.id)
			else
				select
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.cooking_time
				from
					products
				where
					products.prod_name like @name + '%' and products.prod_type = @type
										and
					exists
					(select
						orders_products.is_cooked
					from
						orders_products
					where
						orders_products.order_id = @order_id
						and
						orders_products.product_id = products.id)
		end
		else
		begin
			if (@type = '')
				select distinct
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.prod_type,
					products.cooking_time
				from
					products
				left join crafts on crafts.prod_id = products.id
				where
					products.prod_name like @name + '%'
					and
					(select
						sum(ingredients_coming.ingr_count)
					from
						ingredients_coming
					where
						ingredients_coming.ingr_id = crafts.ingr_id)
					-
					(select
						sum(ingredients_using.ingr_count)
					from
						ingredients_using
					where
						ingredients_using.ingr_id = crafts.ingr_id)
					- crafts.ingr_count > 0
										and
					exists
					(select
						orders_products.is_cooked
					from
						orders_products
					where
						orders_products.order_id = @order_id
						and
						orders_products.product_id = products.id)

			else
				select distinct
					products.id,
					products.prod_name,
					products.price,
					products.img_path,
					products.prod_type,
					products.cooking_time
				from
					products
				left join crafts on crafts.prod_id = products.id
				where
					products.prod_name like @name + '%' and products.prod_type = @type
					and
									(select
						sum(ingredients_coming.ingr_count)
					from
						ingredients_coming
					where
						ingredients_coming.ingr_id = crafts.ingr_id)
					-
					(select
						sum(ingredients_using.ingr_count)
					from
						ingredients_using
					where
						ingredients_using.ingr_id = crafts.ingr_id)
					- crafts.ingr_count > 0
										and
					exists
					(select
						orders_products.is_cooked
					from
						orders_products
					where
						orders_products.order_id = @order_id
						and
						orders_products.product_id = products.id)
		end
	end
GO
/****** Object:  StoredProcedure [dbo].[get_products_types]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[get_products_types] as

	select
		products_types.prod_type_name
	from
		products_types
GO
/****** Object:  StoredProcedure [dbo].[get_units]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[get_units] as
	select	
		units.short_name,
		units.full_name
	from 
		units
GO
/****** Object:  StoredProcedure [dbo].[order_products]    Script Date: 02.04.2023 19:22:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[order_products] @login nvarchar(100) as
	
	select
		orders_products.product_id,
		orders_products.is_cooked
	from
		orders_products
	where
		orders_products.order_id = (select top(1)
										id
									from orders
									order by
										id desc)
GO

grant select, insert on orders to client
go
grant select, insert on orders_products to client
go
grant select on products to client
go
grant select on products_types to client
go
grant exec on get_products to client
go
grant exec on get_products_types to client
go

USE [master]
GO
ALTER DATABASE [Coffe] SET  READ_WRITE 
GO
