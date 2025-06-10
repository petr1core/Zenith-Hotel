// TestReviewComponent.js
const TestReviewComponent = () => {
  const [status, setStatus] = useState("Approved");
  const reviewId = "52ad6bef-test-4fe7-88fd-0fc8c7060294"; // Пример ID // uuidv4()

  const handleModerate = async () => {
    await apiService.put(`/reviews/moderate/${reviewId}`, { status });
  };

  return (
    <div>
      <select value={status} onChange={(e) => setStatus(e.target.value)}>
        <option value="Approved">Approved</option>
        <option value="Rejected">Rejected</option>
      </select>
      <button onClick={handleModerate}>Moderate Review</button>
    </div>
  );
};
export default TestReviewComponent;