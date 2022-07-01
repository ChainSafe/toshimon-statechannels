using LINQtoCSV;

public class MovesDb {

	IEnumerable<MoveRecord> moves;

	public MovesDb(string filePath) {
		try {
			CsvFileDescription inputFileDescription = new CsvFileDescription
			{
			    SeparatorChar = ';', 
			    FirstLineHasColumnNames = true,
			    IgnoreUnknownColumns = true,
			};
			CsvContext cc = new CsvContext();
			moves =
	    		cc.Read<MoveRecord>(filePath, inputFileDescription);
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

	public MoveRecord findByGuid(string guid) {
	    return moves.FirstOrDefault(x => x.Guid == guid);
	}

}