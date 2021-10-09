CREATE PROCEDURE [dbo].[SP_PROXIMO_ID]
@next int OUTPUT
AS
BEGIN
	SET @next = (SELECT MAX(id_producto)+1  FROM T_PRODUCTOS);
END

CREATE PROCEDURE [dbo].[SP_INSERTAR_MAESTRO] 
	@cliente varchar(255), 
	@dto numeric(5,2),
	@total numeric(8,2),
	@presupuesto_nro int OUTPUT
AS
BEGIN
	INSERT INTO T_PRESUPUESTOS(fecha, cliente, descuento, total)
    VALUES (GETDATE(), @cliente, @dto, @total);
    --Asignamos el valor del último ID autogenerado (obtenido --  
    --mediante la función SCOPE_IDENTITY() de SQLServer)	
    SET @presupuesto_nro = SCOPE_IDENTITY();

END

CREATE PROCEDURE [dbo].[SP_CONSULTAR_PRODUCTOS]
AS
BEGIN
	
	SELECT * from T_PRODUCTOS;
END

CREATE PROCEDURE [dbo].[SP_CONSULTAR_PRESUPUESTOS] 
	@fecha_desde Date,
	@fecha_hasta Date,
	@cliente varchar(255),
	@datos_baja varchar(1)
AS
BEGIN
	SELECT * FROM T_PRESUPUESTOS
	WHERE 
	 ((@fecha_desde is null and @fecha_hasta is  null) OR (fecha between @fecha_desde and @fecha_hasta))
	 AND(@cliente is null OR (cliente like '%' + @cliente + '%'))
	 AND (@datos_baja is null OR (@datos_baja = 'S') OR (@datos_baja = 'N' and fecha_baja IS  NULL))
	 
END

CREATE PROCEDURE [dbo].[SP_CONSULTAR_PRESUPUESTO_POR_ID]
	@nro int	
AS
BEGIN
	SELECT *
	FROM T_PRESUPUESTOS t, T_DETALLES_PRESUPUESTO t1, T_PRODUCTOS t2
	WHERE t.presupuesto_nro = t1.presupuesto_nro
	AND t1.id_producto = t2.id_producto
	AND t.presupuesto_nro = @nro;
END

CREATE PROCEDURE [dbo].[SP_INSERTAR_DETALLE] 
	@presupuesto_nro int,
	@detalle int, 
	@id_producto int, 
	@cantidad int
AS
BEGIN
	INSERT INTO T_DETALLES_PRESUPUESTO(presupuesto_nro,detalle_nro, id_producto, cantidad)
    VALUES (@presupuesto_nro, @detalle, @id_producto, @cantidad);
  
END

CREATE PROCEDURE [dbo].[SP_REPORTE_PRODUCTOS]
AS
BEGIN
	SELECT t2.n_producto as producto, SUM(t1.cantidad) as cantidad
	FROM T_PRESUPUESTOS t, T_DETALLES_PRESUPUESTO t1, T_PRODUCTOS t2
	WHERE t.presupuesto_nro = t1.presupuesto_nro
	AND t1.id_producto = t2.id_producto
	GROUP BY t2.n_producto 
END

CREATE PROCEDURE SP_CONSULTAR_PRESUPUESTOS
@fecha_desde DATE,
@fecha_hasta DATE,
@cliente VARCHAR(255),
@datos_baja VARCHAR(1)
AS
	BEGIN
		-- Consulta Dinamica
		IF(@datos_baja = 'S')
			SELECT * FROM T_PRESUPUESTOS
			WHERE ( fecha BETWEEN @fecha_desde AND @fecha_hasta )
			AND cliente LIKE '%' + @cliente + '%'
			AND fecha_baja IS NOT NULL
		ELSE
			SELECT * FROM T_PRESUPUESTOS
			WHERE ( fecha BETWEEN @fecha_desde AND @fecha_hasta )
			AND cliente LIKE '%' + @cliente + '%'
			AND fecha_baja IS NULL
	END
EXEC SP_CONSULTAR_PRESUPUESTOS @fecha_desde = '2021-05-01', @fecha_hasta = '2021-10-01', @cliente = 'CONSUMIDOR', @datos_baja = 'N'
EXEC SP_CONSULTAR_PRESUPUESTOS @fecha_desde = '1/5/2021', @fecha_hasta = '01/10/2021', @cliente = 'CONSUMIDOR', @datos_baja = 'N'
SELECT * FROM T_PRESUPUESTOS WHERE ( fecha BETWEEN '1/5/2021' AND '01/10/2021' ) AND cliente LIKE '%CONSUMIDOR%' AND fecha_baja IS NULL

CREATE PROCEDURE SP_CONSULTAR_DETALLES
@presupuesto_nro INT
AS	
	BEGIN
		SELECT
			@presupuesto_nro AS 'presupuesto_nro',
			DP.detalle_nro,
			P.n_producto,
			P.precio,
			DP.cantidad
		FROM T_DETALLES_PRESUPUESTO DP
		INNER JOIN T_PRODUCTOS P ON DP.id_producto = P.id_producto
		WHERE DP.presupuesto_nro = @presupuesto_nro
	END
EXECUTE SP_CONSULTAR_DETALLES @presupuesto_nro = 3

CREATE PROCEDURE SP_ELIMINAR_PRESUPUESTO
@presupuesto_nro INT
AS
	UPDATE T_PRESUPUESTOS
		SET fecha_baja = CONVERT(varchar, GETDATE(), 23)
	WHERE presupuesto_nro = @presupuesto_nro
	
EXEC SP_ELIMINAR_PRESUPUESTO @presupuesto_nro = 7 

CREATE PROCEDURE SP_ELIMINAR_DETALLE_PRESUPUESTO
@presupuesto_nro INT
AS
	BEGIN
		DELETE FROM T_DETALLES_PRESUPUESTO WHERE presupuesto_nro = @presupuesto_nro
	END
EXEC SP_ELIMINAR_DETALLE_PRESUPUESTO @presupuesto_nro = 7

SELECT * FROM T_PRESUPUESTOS
SELECT * FROM T_DETALLES_PRESUPUESTO
DELETE FROM T_PRESUPUESTOS WHERE presupuesto_nro = 6

CREATE PROCEDURE SP_CARGAR_PRESUPUESTOS
@presupuesto_nro INT
AS
	BEGIN
		SELECT * FROM T_PRESUPUESTOS WHERE presupuesto_nro = @presupuesto_nro
	END

EXEC SP_CARGAR_PRESUPUESTOS @presupuesto_nro = 2

CREATE PROCEDURE SP_CARGAR_DETALLES
@presupuesto_nro INT
AS
	BEGIN
		SELECT
			DP.presupuesto_nro,
			DP.detalle_nro,
			P.id_producto,
			P.n_producto,
			P.precio,
			DP.cantidad
		FROM T_DETALLES_PRESUPUESTO DP
		INNER JOIN T_PRODUCTOS P ON DP.id_producto = P.id_producto
		WHERE presupuesto_nro = @presupuesto_nro
	END

EXEC SP_CARGAR_DETALLES @presupuesto_nro = 9

CREATE PROCEDURE SP_UPDATE_PRESUPUESTOS
@nro_presupuesto INT,
@cliente varchar(255), 
@dto numeric(5,2),
@total numeric(8,2)
AS
	BEGIN
		UPDATE T_PRESUPUESTOS
			SET fecha = GETDATE(),
			cliente = @cliente,
			descuento = @dto,
			total = @total,
			fecha_baja = NULL
		WHERE presupuesto_nro = @nro_presupuesto
	END


CREATE PROCEDURE SP_UPDATE_DETALLES_PRESUPUESTO
@nro_presupuesto INT,
@detalle int, 
@id_producto int, 
@cantidad int,
@retorno int = NULL
AS
	BEGIN
		SELECT @retorno = detalle_nro FROM T_DETALLES_PRESUPUESTO 
		WHERE detalle_nro = @detalle AND presupuesto_nro = @nro_presupuesto
		IF(@retorno IS NOT NULL) 
			UPDATE T_DETALLES_PRESUPUESTO
			SET id_producto = @id_producto,
			cantidad = @cantidad
			WHERE presupuesto_nro = @nro_presupuesto AND detalle_nro = @detalle
		ELSE
			EXEC SP_INSERTAR_DETALLE @presupuesto_nro=@nro_presupuesto, @detalle=@detalle, @id_producto=@id_producto, @cantidad=@cantidad
	END

SELECT * FROM T_PRESUPUESTOS
SELECT * FROM T_DETALLES_PRESUPUESTO
SELECT detalle_nro FROM T_DETALLES_PRESUPUESTO WHERE detalle_nro = 2 AND presupuesto_nro = 9

CREATE PROCEDURE [dbo].[SP_REPORTE_PRODUCTOS]
AS
BEGIN
	SELECT t2.n_producto as producto, SUM(t1.cantidad) as cantidad
	FROM T_PRESUPUESTOS t, T_DETALLES_PRESUPUESTO t1, T_PRODUCTOS t2
	WHERE t.presupuesto_nro = t1.presupuesto_nro
	AND t1.id_producto = t2.id_producto
	GROUP BY t2.n_producto 
END

CREATE PROCEDURE [dbo].[SP_CONSULTAR_PRESUPUESTO_POR_ID]
	@nro int	
AS
BEGIN
	SELECT *
	FROM T_PRESUPUESTOS t, T_DETALLES_PRESUPUESTO t1, T_PRODUCTOS t2
	WHERE t.presupuesto_nro = t1.presupuesto_nro
	AND t1.id_producto = t2.id_producto
	AND t.presupuesto_nro = @nro;
END

CREATE PROCEDURE [dbo].[SP_REGISTRAR_BAJA_PRESUPUESTOS] 
	@presupuesto_nro int
AS
BEGIN
	UPDATE T_PRESUPUESTOS SET fecha_baja = GETDATE()
	WHERE presupuesto_nro = @presupuesto_nro;
END