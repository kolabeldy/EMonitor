CREATE VIEW ViewMains AS
SELECT 
	Id,
	Period, 
	IdEnergyResource, 
	IdCostCenter, 
	SUM(Plan) AS Plan, 
	SUM(Fact) AS Fact
FROM EnergyMonthUses
WHERE IdOrganization = 2011
GROUP BY Period, IdEnergyResource, IdCostCenter