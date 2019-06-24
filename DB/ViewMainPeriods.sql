CREATE VIEW ViewMainPeriods AS
SELECT 
	Id,
	Period,
	substr(Period,-4) AS Year,
	substr(Period,1,length(Period)-5) AS Month,
	IdEnergyResource, 
	IdCostCenter, 
	Plan, 
	Fact
FROM ViewMains