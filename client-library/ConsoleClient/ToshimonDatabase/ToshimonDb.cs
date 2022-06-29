using LINQtoCSV;

public class ToshimonDb {

	IEnumerable<MonsterRecord> monsters;

	public ToshimonDb(string filePath) {
		try {
			CsvFileDescription inputFileDescription = new CsvFileDescription
			{
			    SeparatorChar = ';', 
			    FirstLineHasColumnNames = true
			};
			CsvContext cc = new CsvContext();
			monsters =
	    		cc.Read<MonsterRecord>(filePath, inputFileDescription);
		} 
		catch(AggregatedException ae)
	    {
	        // Process all exceptions generated while processing the file
	        List<Exception> innerExceptionsList =
	            (List<Exception>)ae.Data["InnerExceptionsList"];
	        foreach (Exception e in innerExceptionsList)
	        {
	            Console.WriteLine(e.Message);
	        }
	    }
		catch(Exception e) {
	        Console.WriteLine(e.Message);
	    }

	}

	public MonsterRecord findByToshidexNumber(int toshidexNumber) {
		return monsters.FirstOrDefault(x => x.ToshidexNumber == toshidexNumber);
	}

}