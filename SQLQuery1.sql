

SELECT count(*) FROM Equipment JOIN Carton ON EQUIPMENT.ID=CARTON.CartonNumber

SELECT CartonNumber,EquipmentId,ModelTypeId,SerialNumber FROM CartonDetail cd JOIN EQUIPMENT EQ ON CD.EquipmentId=EQ.Id JOIN Carton C ON CD.CartonId=C.Id

SELECT *  FROM  Carton 
SELECT * FROM cartondetail

select * from Equipment