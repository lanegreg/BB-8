# Car Research Section #


### Make Page ###
> http://car.com**/{make}/**
>
> URL Consraints:
> 
	- {make} must be *ACTIVE*
> 
>> Links on Page:
>>
	- Has *NEW* Make-SuperModel Link Types Like:
		<a href='/{make}/{super-model}/' />

>>Example:
>> 
	- Page URL:
		http://car.com/ford/
>> 
	- Make-SuperModel Links on Page:
		<a href='/ford/mustang/' />
		<a href='/ford/fiesta/' />
		<a href='/ford/explorer/' />



### SuperModel Page ###
> http://car.com**/{make}/{super-model}/** 
>
> URL Consraints:
> 
	- {make} must be *ACTIVE*
	- {super-model} must have at least one *NEW* trim.
> 
>> Links on Page:
>>
	- Has Make-SuperModel-Year Link Types Like:
		<a href='/{make}/{super-model}/{year}/' />
>>
	- Has *NEW* Make-SuperModel-Year-Trim Link Types Like:
		<a href='/{make}/{super-model}/{year}/{trim}/' />

>> Example:
>> 
	- Page URL:
		http://car.com/ford/mustang/
>> 
	- Make-SuperModel-Year Links on Page:
		<a href='/ford/mustang/2016/' />
		<a href='/ford/mustang/2015/' />
		<a href='/ford/mustang/2014/' />
>> 
	- Make-SuperModel-Year-Trim Links on Page:
		<a href='/ford/mustang/2016/gt/' />
		<a href='/ford/mustang/2016/gt-premium/' />
		<a href='/ford/mustang/2016/ecoboost/' />
		<a href='/ford/mustang/2016/v6/' />
		<a href='/ford/mustang/2015/gt/' />
		<a href='/ford/mustang/2015/gt-premium/' />
		<a href='/ford/mustang/2015/ecoboost/' />
		<a href='/ford/mustang/2015/v6/' />



### Year Page ###
> http://car.com**/{make}/{super-model}/{year}/** 
>
> URL Consraints:
> 
	- {make} must be *ACTIVE*
	- {super-model} has NO constraint.
	- {year} must be greater than or equal to 2007.
> 
>> Links on Page:
>>
	- Has Make-SuperModel-Year-Trim Link Types Like:
		<a href='/{make}/{super-model}/{year}/{trim}/' />

>> Example:
>> 
	- Page URL:
		http://car.com/ford/mustang/2015
>> 
	- Make-SuperModel-Year-Trim Links on Page:
		<a href='/ford/mustang/2015/gt/' />
		<a href='/ford/mustang/2015/gt-premium/' />
		<a href='/ford/mustang/2015/ecoboost/' />
		<a href='/ford/mustang/2015/v6/' />



### Trim Page (Overview) ###
> http://car.com**/{make}/{super-model}/{year}/{trim}/** 
>
> URL Consraints:
> 
	- {make} must be *ACTIVE*
	- {super-model} has NO constraint.
	- {year} must be greater than or equal to 2007.
	- {trim} must be *ACTIVE*
> 
>> Links on Page:
>>
	- Has Make-SuperModel-Year-Trim-Section Link Types Like:
		<a href='/{make}/{super-model}/{year}/{trim}/{trim-section}/' />

>> Example:
>> 
	- Page URL:
		http://car.com/ford/mustang/2015/gt/
>> 
	- Make-SuperModel-Year-Trim-Section Links on Page:
		<a href='/ford/mustang/2015/gt/specifications/' />
		<a href='/ford/mustang/2015/gt/prices/' />
		<a href='/ford/mustang/2015/gt/incentives/' />
		<a href='/ford/mustang/2015/gt/safety/' />
		<a href='/ford/mustang/2015/gt/warranty/' />

